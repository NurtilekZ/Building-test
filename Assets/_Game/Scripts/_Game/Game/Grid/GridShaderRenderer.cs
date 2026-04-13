using UnityEngine;

namespace Game.Grid
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public class GridShaderRenderer : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private bool updateEveryFrame = true;

        [Header("Shader Properties")]
        [SerializeField] private string originProperty = "_GridOrigin";
        [SerializeField] private string sizeProperty = "_GridSize";
        [SerializeField] private string cellSizeProperty = "_CellSize";

        private Renderer _targetRenderer;
        private MaterialPropertyBlock _propertyBlock;

        private void Awake()
        {
            _targetRenderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            if (gridManager)
            {
                gridManager.GridRebuilt.AddListener(ApplyGridToMaterial);
            }

            ApplyGridToMaterial();
        }

        private void OnDisable()
        {
            if (gridManager)
            {
                gridManager.GridRebuilt.RemoveListener(ApplyGridToMaterial);
            }
        }

        private void LateUpdate()
        {
            if (updateEveryFrame)
            {
                ApplyGridToMaterial();
            }
        }

        [ContextMenu("Apply Grid To Shader")]
        public void ApplyGridToMaterial()
        {
            if (!_targetRenderer)
            {
                _targetRenderer = GetComponent<Renderer>();
            }

            if (!gridManager || !_targetRenderer)
            {
                return;
            }

            _targetRenderer.GetPropertyBlock(_propertyBlock);

            Vector3 origin = gridManager.Origin;
            Vector2 worldSize = new Vector2(
                gridManager.Width * gridManager.CellSize,
                gridManager.Height * gridManager.CellSize);
            
            transform.localScale = new Vector3(gridManager.Width * 0.4f, 1, gridManager.Height * 0.4f) * (gridManager.CellSize / 4) ;
            transform.position = new Vector3(origin.x + gridManager.Width / 2.0f * gridManager.CellSize, origin.y, origin.z + gridManager.Height / 2.0f * gridManager.CellSize);

            _propertyBlock.SetVector(originProperty, new Vector4(origin.x, origin.y, origin.z, 0f));
            _propertyBlock.SetVector(sizeProperty, new Vector4(worldSize.x, worldSize.y, 0f, 0f));
            _propertyBlock.SetFloat(cellSizeProperty, gridManager.CellSize);

            _targetRenderer.SetPropertyBlock(_propertyBlock);
        }
    }
}
