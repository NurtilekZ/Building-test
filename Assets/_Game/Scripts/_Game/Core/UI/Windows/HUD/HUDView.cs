using System;
using Core.Initialization.Services;
using Core.UI.CoreMVP;
using Game.Buildings.Data;
using Game.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Core.UI.Windows.HUD
{
    public class HUDView : View<HUDModel>
    {
        [SerializeField] private Button _mapBtn;
        [SerializeField] private GridUIRenderer _gridUIRenderer;
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _rotateR;
        [SerializeField] private InputActionReference _rotateL;
        [SerializeField] private Toggle _moveIndicators;
        [SerializeField] private Toggle _rotateRIndicator;
        [SerializeField] private Toggle _rotateLIndicator;
        
        public event Action OnMapBtnClick;

        private readonly IBuildingService _buildingService = ServiceLocator.Instance.Get<IBuildingService>();

        private void OnEnable()
        {
            _gridUIRenderer.RefreshGrid();
            _buildingService.OnBuildingPlaced += RefreshGrid;
            _moveAction.action.started += OnMove;
            _moveAction.action.canceled += OnMove;
            _rotateR.action.started += OnRotateR;
            _rotateR.action.canceled += OnRotateR;
            _rotateL.action.started += OnRotateL;
            _rotateL.action.canceled += OnRotateL;
        }

        private void OnDisable()
        {
            _buildingService.OnBuildingPlaced -= RefreshGrid;
            _moveAction.action.started -= OnMove;
            _moveAction.action.canceled -= OnMove;
            _rotateR.action.started -= OnRotateR;
            _rotateR.action.canceled -= OnRotateR;
            _rotateL.action.started -= OnRotateL;
            _rotateL.action.canceled -= OnRotateL;
        }

        private void RefreshGrid(BuildingData arg1, Vector2Int arg2)
        {
            _gridUIRenderer.RefreshGrid();
        }

        private void Awake()
        {
            _mapBtn.onClick.AddListener(() => OnMapBtnClick?.Invoke());
        }

        private void Start()
        {
            _gridUIRenderer.RefreshGrid();
        }

        protected override void Render()
        {
            _gridUIRenderer.RefreshGrid();
        }

        private void OnRotateL(InputAction.CallbackContext obj)
        {
            _rotateLIndicator.isOn = obj.action.WasPressedThisFrame();
        }

        private void OnRotateR(InputAction.CallbackContext obj)
        {
            _rotateRIndicator.isOn = obj.action.WasPressedThisFrame();
        }

        private void OnMove(InputAction.CallbackContext obj)
        {
            _moveIndicators.isOn = obj.action.WasPressedThisFrame();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _mapBtn.onClick.RemoveAllListeners();
        }
    }
}