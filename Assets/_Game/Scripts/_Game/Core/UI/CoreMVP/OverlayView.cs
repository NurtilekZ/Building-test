using UnityEngine;

namespace Core.UI.CoreMVP
{
    public abstract class OverlayView<TModel> : View<TModel> where TModel : Model
    {
        [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2f, 0f);
        [SerializeField] private bool hideWhenOffscreen = true;

        private Transform _targetTransform;
        private Camera _targetCamera;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        protected virtual void Awake()
        {
            _targetCamera = Camera.main;
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Bind(Transform targetTransform)
        {
            _targetTransform = targetTransform;
        }

        private void LateUpdate()
        {
            if (!_targetTransform || !_targetCamera)
            {
                return;
            }

            Vector3 screenPoint = _targetCamera.WorldToScreenPoint(_targetTransform.position + worldOffset);
            bool visible = screenPoint.z > 0f;

            if (hideWhenOffscreen && !visible)
            {
                SetAlpha(0f);
                return;
            }

            if (_rectTransform)
            {
                _rectTransform.position = screenPoint;
            }
            else
            {
                transform.position = screenPoint;
            }

            SetAlpha(1f);
        }

        private void SetAlpha(float value)
        {
            if (_canvasGroup)
            {
                _canvasGroup.alpha = value;
                _canvasGroup.blocksRaycasts = value > 0f;
                _canvasGroup.interactable = value > 0f;
            }
        }

        protected override void Render()
        {
            
        }
    }
}
