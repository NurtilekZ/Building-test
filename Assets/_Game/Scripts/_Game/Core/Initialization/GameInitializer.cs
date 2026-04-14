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
using UnityEngine;
using UnityEngine.InputSystem;

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
        [SerializeField] private InputActionAsset _inputActionAsset;
        
        [Header("Building System")]
        [SerializeField] private BuildingPlacementManager _placementManager;
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
        private IGridService _gridService;
        private IUIService _uiService;
        private IBuildingService _buildingService;
        private ISaveLoadService _saveLoadService;

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
            
            if (_openInitialWindow && _windowManager)
            {
                _windowManager.OpenWindow(_initialWindowId, new HUDModel(), _closeOtherWindows);
            }

            _placementManager.BindServices(_uiService, _gridService, _saveLoadService);
            
            ServiceLocator.Instance.Get<ISaveLoadService>().TryLoadFromStartupMode(_loadMode);
            
            _inputActionAsset.Enable();
            
            _hasInitialized = true;
        }

        private void RegisterServices()
        {
            ServiceLocator locator = ServiceLocator.Instance;

            _uiService = locator.Register<IUIService>(new UIService(_windowManager, _buildingsList));
            _gridService = locator.Register<IGridService>(new GridService(gridManager));
            _buildingService = locator.Register<IBuildingService>(new BuildingService(_placementManager, _buildingsList));
            _saveLoadService = locator.Register<ISaveLoadService>(new SaveLoadService(gridManager, _placementManager, _buildingsList, _clearSavesOnLoad));
        }

        public void SaveBuildings()
        {
            ServiceLocator.Instance.Get<ISaveLoadService>().SaveBuildings();
        }

        private void OnApplicationQuit()
        {
            _inputActionAsset.Disable();
            SaveBuildings();
        }
    }
}
