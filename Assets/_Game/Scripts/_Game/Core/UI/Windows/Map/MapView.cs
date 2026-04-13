using System;
using Core.Initialization.Services;
using Core.UI.CoreMVP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Windows.Map
{
    public class MapView : View<MapModel>
    {
        [Header("Grid Renderer")]
        [SerializeField] private RawImage gridImage;
        
        [Header("Settings")]
        [SerializeField] private Button closeButton;

        [Header("Visual Settings")] 
        [SerializeField] private Color emptyColor;
        [SerializeField] private Color occupiedColor = new(0.4f, 0.6f, 0.4f, 0.8f);
        [SerializeField] private Color gridLineColor = new(1f, 1f, 1f, 1f);
        [SerializeField] [Range(0.0001f, 0.5f)]private float _lineThickness = 0.2f;
        

        public event Action OnCloseBtnPress;

        private IGridService _gridService;
        
        private Material _gridMaterial;
        private Texture2D _occupancyTexture;
        private int _cachedWidth = -1;
        private int _cachedHeight = -1;
        
        private void Awake()
        {
            closeButton.onClick.AddListener(() => OnCloseBtnPress?.Invoke());
        }

        private void SetupShader()
        {
            if (!gridImage) return;

            Shader shader = Shader.Find("UI/GridOccupancy");
            
            if (!shader)
            {
                Debug.LogError("MapView: Could not find 'UI/URP/GridShader'. Make sure the shader is in the project.");
                return;
            }

            _gridMaterial = new Material(shader);
            gridImage.material = _gridMaterial;
        }

        protected override void Render()
        {
            _gridService = ServiceLocator.Instance.Get<IGridService>();

            if (!_gridMaterial)
            {
                SetupShader();
                InitializeGrid();
            }
            else
            { 
                RefreshOccupancy();
            }
        }

        private void InitializeGrid()
        {
            if (_gridService == null || !_gridMaterial)
            {
                return;
            }

            int width = _gridService.Width;
            int height = _gridService.Height;

            if (width != _cachedWidth || height != _cachedHeight)
            {
                if (_occupancyTexture)
                {
                    Destroy(_occupancyTexture);
                }

                _occupancyTexture = new Texture2D(width, height, TextureFormat.R8, false);
                _occupancyTexture.filterMode = FilterMode.Point;
                _occupancyTexture.wrapMode = TextureWrapMode.Clamp;

                _cachedWidth = width;
                _cachedHeight = height;

                RefreshOccupancy();
            }
        }

        private void UpdateShaderProperties()
        {
            if (!_gridMaterial || _gridService == null)
            {
                return;
            }

            _gridMaterial.SetColor("_BaseColor", emptyColor);
            _gridMaterial.SetColor("_LineColor", gridLineColor);
            _gridMaterial.SetColor("_OccupiedColor", occupiedColor);
            _gridMaterial.SetFloat("_LineThickness", _lineThickness);
            _gridMaterial.SetVector("_CellCount", new Vector2(_gridService.Width, _gridService.Height));

            if (_occupancyTexture)
            {
                _gridMaterial.SetTexture("_OccupancyTex", _occupancyTexture);
            }
        }

        private void UpdateOccupancyTexture()
        {
            if (!_occupancyTexture || _gridService == null)
            {
                return;
            }

            int width = _gridService.Width;
            int height = _gridService.Height;
            Color[] colors = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2Int cell = new Vector2Int(x, y);
                    bool isOccupied = _gridService.IsOccupied(cell);
                    colors[y * width + x] = isOccupied ? occupiedColor : emptyColor;
                }
            }

            _occupancyTexture.Reinitialize(width, height);
            _occupancyTexture.SetPixels(colors);
            _occupancyTexture.Apply();
            
            gridImage.rectTransform.sizeDelta = new Vector2(width , height) * _gridService.CellSize * 10;
            gridImage.rectTransform.anchoredPosition = new Vector2((_gridService.Origin.x + width * 2) * 10, (_gridService.Origin.z + height * 2) * 10 - 8);
        }

        public void RefreshOccupancy()
        {
            UpdateOccupancyTexture();
            UpdateShaderProperties();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            closeButton.onClick.RemoveAllListeners();
            if (_occupancyTexture != null)
            {
                Destroy(_occupancyTexture);
            }
            if (_gridMaterial != null)
            {
                Destroy(_gridMaterial);
            }
        }
    }
}
