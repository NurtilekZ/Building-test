using System;
using Core.UI.CoreMVP;
using Game.Grid;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Windows.Map
{
    public class MapView : View<MapModel>
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private GridUIRenderer _gridUIRenderer;

        public override event Action OnCloseClicked;
        
        private void Awake()
        {
            closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
        }

        protected override void Render()
        {
            _gridUIRenderer.RefreshGrid();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            closeButton.onClick.RemoveAllListeners();
        }
    }
}
