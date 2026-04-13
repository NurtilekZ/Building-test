using System;
using Core.UI.CoreMVP;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Windows.HUD
{
    public class HUDView : View<HUDModel>
    {
        [SerializeField] private TMP_Text _currencyBalanceTxt;
        [SerializeField] private Button _mapBtn;

        public event Action OnMapBtnClick;

        private void Awake()
        {
            _mapBtn.onClick.AddListener(() => OnMapBtnClick?.Invoke());
        }

        protected override void Render()
        {
            Model.Balance.Subscribe(x => _currencyBalanceTxt.text = $"Balance : {x.ToString()}").AddTo(_disposableList);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _mapBtn.onClick.RemoveAllListeners();
        }
    }
}