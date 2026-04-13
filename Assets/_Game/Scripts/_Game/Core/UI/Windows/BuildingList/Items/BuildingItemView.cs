using System;
using Core.UI.CoreMVP;
using Game.Buildings.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Windows.BuildingList.Items
{
    public class BuildingItemView : SubView<BuildingData>
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Button _button;
        private BuildingData _data;

        public event Action<BuildingData> OnBtnClick;
        
        private void Awake()
        {
            _button.onClick.AddListener(() => OnBtnClick?.Invoke(_data));
        }

        public override void BindData(BuildingData data)
        {
            _data = data;
            _icon.sprite = data.Icon[0];
            _name.text = data.DisplayName;
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}