using Core.Initialization.SaveLoad;
using Core.Initialization.Services;
using Core.Initialization.Services.Grid;
using Core.Initialization.Services.SaveLoad;
using Core.Initialization.Services.UI;
using Core.UI.Windows;
using Core.UI.Windows.HUD;
using Game.Assets;
using Game.Buildings.Core;
using Game.Grid;
using R3;
using UnityEngine;

namespace Core.Initialization
{
    [DisallowMultipleComponent]
    public class GameInitializer : MonoBehaviour
    {
        public enum StartupLoadMode
        {
            None = 0,
            LoadIfSaveExists = 1,
            ForceLoad = 2
        }
        
        [Header("Core References")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private UIWindowManager _windowManager;
        
        [Header("Building System")]
        [SerializeField] private BuildingPlacementManager _placementManager;
        [SerializeField] private BuildingInteractor _buildingInteractor;
        [SerializeField] private BuildingsListSO _buildingsList;
        
        [Header("Startup Flow")]
        [SerializeField] private StartupLoadMode _loadMode = StartupLoadMode.LoadIfSaveExists;
        [SerializeField] private bool _clearSavesOnLoad;
        [SerializeField] private bool _initializeGridFirst = true;
        [SerializeField] private bool _registerServices = true;
        
        [Header("Initial Window")]
        [SerializeField] private bool _openInitialWindow;
        [SerializeField] private WindowID _initialWindowId = WindowID.HUD;
        [SerializeField] private bool _closeOtherWindows = true;
        
        private bool _hasInitialized;
        
        private void Start()
        {
            InitializeGameSystems();
        }
        
        private void OnDestroy()
        {
            if (_hasInitialized)
            {
                ServiceLocator.Shutdown();
            }
        }
        
        [ContextMenu("Initialize Game Systems")]
        public void InitializeGameSystems()
        {
            if (_hasInitialized)
            {
                return;
            }

            if (_registerServices)
            {
                RegisterServices();
            }

            if (_initializeGridFirst && gridManager)
            {
                gridManager.InitializeGrid();
            }

            ServiceLocator.Instance.Get<ISaveLoadService>().TryLoadFromStartupMode(_loadMode);

            if (_openInitialWindow && _windowManager)
            {
                _windowManager.OpenWindow(_initialWindowId, new HUDModel { Balance = new ReactiveProperty<int>(1000) }, _closeOtherWindows);
            }

            _hasInitialized = true;
        }

        private void RegisterServices()
        {
            ServiceLocator locator = ServiceLocator.Instance;

            locator.Register<IGridService>(new GridService(gridManager));
            locator.Register<IBuildingService>(new BuildingService(_placementManager, _buildingsList, _buildingInteractor));
            locator.Register<ISaveLoadService>(new SaveLoadService(gridManager, _placementManager, _buildingsList, _clearSavesOnLoad));
            locator.Register<IUIService>(new UIService(_windowManager, _buildingsList));
        }

        [ContextMenu("Save Buildings")]
        public void SaveBuildings()
        {
            ServiceLocator.Instance.Get<ISaveLoadService>().SaveBuildings();
        }
        
        [ContextMenu("Load Buildings")]
        public void LoadBuildings()
        {
            ServiceLocator.Instance.Get<ISaveLoadService>().LoadBuildings();
        }


        private void OnApplicationQuit()
        {
            SaveBuildings();
        }
    }
}
