using System;
using Core.UI.CoreMVP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Windows.BuildingInfo
{
    public class BuildingInfoView : View<BuildingInfoModel>
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _currentLevelText;
        [SerializeField] private Image _image;
        [SerializeField] private Button _closeBtn;
        
        public event Action OnCloseClick;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(() => OnCloseClick?.Invoke());
        }

        protected override void Render()
        {
            var buildingData = Model.Building.Data;
            _titleText.text = buildingData.DisplayName;
            _descriptionText.text = buildingData.Description;
            var currentLevel = Model.Building.CurrentLevel;
            var maxLevel = buildingData.MaxLevel;
            _currentLevelText.text = $"Level {(currentLevel == maxLevel ? "MAX" : maxLevel.ToString())}";
            _image.sprite = buildingData.Icon[currentLevel - 1];
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _closeBtn.onClick.RemoveAllListeners();
        }
    }
}