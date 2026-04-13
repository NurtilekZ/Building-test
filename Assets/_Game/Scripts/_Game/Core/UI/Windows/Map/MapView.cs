using System;
using Core.Initialization.Services;
using Core.UI.CoreMVP;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Windows.Map
{
    public class MapView : View<MapModel>
    {
        [Header("Grid Renderer")]
        [SerializeField] private RawImage gridImage;
        [SerializeField] private AspectRatioFitter aspectRatioFitter;
        
        [Header("Settings")]
        [SerializeField] private Button closeButton;
        
        [Header("Visual Settings")]
        [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        [SerializeField] private Color occupiedColor = new Color(0.4f, 0.6f, 0.4f, 0.8f);
        [SerializeField] private Color selectedColor = new Color(0.2f, 0.6f, 0.9f, 0.9f);
        [SerializeField] private Color gridLineColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        [SerializeField] [Range(0.01f, 0.1f)] private float gridLineWidth = 0.02f;

        public event Action OnCloseBtnPress;

        private IGridService _gridService;
        private Material _gridMaterial;
        private Texture2D _occupancyTexture;
        private Vector2Int _selectedCell = new Vector2Int(-1, -1);
        private int _cachedWidth = -1;
        private int _cachedHeight = -1;

        private void Awake()
        {
            closeButton.onClick.AddListener(() => OnCloseBtnPress?.Invoke());
        }

        private void SetupShader()
        {
            if (gridImage == null) return;

            Shader shader = Shader.Find("UI/URP/GridShader");
            if (shader == null)
            {
                shader = Shader.Find("UI/GridShader");
            }
            
            if (shader == null)
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
            // SetupShader();
            // InitializeGrid();
        }

        private void InitializeGrid()
        {
            if (_gridService == null || _gridMaterial == null)
            {
                return;
            }

            int width = _gridService.Width;
            int height = _gridService.Height;

            if (width != _cachedWidth || height != _cachedHeight)
            {
                if (_occupancyTexture != null)
                {
                    Destroy(_occupancyTexture);
                }

                _occupancyTexture = new Texture2D(width, height, TextureFormat.R8, false);
                _occupancyTexture.filterMode = FilterMode.Point;
                _occupancyTexture.wrapMode = TextureWrapMode.Clamp;

                _cachedWidth = width;
                _cachedHeight = height;

                UpdateShaderProperties();
                UpdateOccupancyTexture();
            }

            if (aspectRatioFitter != null)
            {
                aspectRatioFitter.aspectRatio = (float)width / height;
            }
        }

        private void UpdateShaderProperties()
        {
            if (_gridMaterial == null || _gridService == null)
            {
                return;
            }

            _gridMaterial.SetInt("_GridWidth", _gridService.Width);
            _gridMaterial.SetInt("_GridHeight", _gridService.Height);
            _gridMaterial.SetColor("_BaseColor", Color.white);
            _gridMaterial.SetColor("_EmptyColor", emptyColor);
            _gridMaterial.SetColor("_OccupiedColor", occupiedColor);
            _gridMaterial.SetColor("_SelectedColor", selectedColor);
            _gridMaterial.SetColor("_GridLineColor", gridLineColor);
            _gridMaterial.SetFloat("_GridLineWidth", gridLineWidth);
            _gridMaterial.SetInt("_SelectedCellX", _selectedCell.x);
            _gridMaterial.SetInt("_SelectedCellY", _selectedCell.y);
            _gridMaterial.SetFloat("_UseTexture", _occupancyTexture != null ? 1f : 0f);

            if (_occupancyTexture != null)
            {
                _gridMaterial.SetTexture("_CellOccupancyTex", _occupancyTexture);
            }
        }

        private void UpdateOccupancyTexture()
        {
            if (_occupancyTexture == null || _gridService == null)
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
                    colors[y * width + x] = isOccupied ? Color.white : Color.black;
                }
            }

            _occupancyTexture.SetPixels(colors);
            _occupancyTexture.Apply();
        }

        public void RefreshOccupancy()
        {
            UpdateOccupancyTexture();
        }

        private void UpdateSelectedCell(Vector2Int cell)
        {
            _selectedCell = cell;
            if (_gridMaterial != null)
            {
                _gridMaterial.SetInt("_SelectedCellX", cell.x);
                _gridMaterial.SetInt("_SelectedCellY", cell.y);
            }
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
