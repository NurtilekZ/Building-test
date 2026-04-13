using System;
using Core.UI.CoreMVP;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Windows.HUD
{
    public class HUDView : View<HUDModel>
    {
        [SerializeField] private Button _mapBtn;

        public event Action OnMapBtnClick;

        private void Awake()
        {
            _mapBtn.onClick.AddListener(() => OnMapBtnClick?.Invoke());
        }

        protected override void Render()
        {
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _mapBtn.onClick.RemoveAllListeners();
        }
    }
}