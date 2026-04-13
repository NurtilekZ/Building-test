using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Movement
{
    public class Move : MonoBehaviour
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference fastMoveAction;
        [SerializeField] private InputActionReference panHoldAction;
        [SerializeField] private InputActionReference lookDeltaAction;
        [SerializeField] private InputActionReference zoomAction;

        [Header("Move")]
        [SerializeField] private float moveSpeed = 12f;
        [SerializeField] private float fastMoveMultiplier = 2f;

        [Header("Mouse Drag Pan")]
        [SerializeField] private bool enableMouseDragPan = true;
        [SerializeField] private float panSpeed = 0.1f;

        [Header("Zoom")]
        [SerializeField] private bool enableZoom = true;
        [SerializeField] private Camera zoomCamera;
        [SerializeField] private float zoomSpeed = 40f;
        [SerializeField] private float zoomInputMultiplier = 1f;
        [SerializeField] private float minOrthographicSize = 6f;
        [SerializeField] private float maxOrthographicSize = 60f;

        [Header("Bounds")]
        [SerializeField] private bool useMovementBounds = true;
        [SerializeField] private Vector2 xBounds = new Vector2(-100f, 100f);
        [SerializeField] private Vector2 zBounds = new Vector2(-100f, 100f);


        private void Awake()
        {
            EnableControls(true);
        }

        private void OnDestroy()
        {
            EnableControls(false);
        }

        private void Update()
        {
            HandleMove();
            HandleMouseDragPan();
            HandleZoom();
            ClampToBounds();
        }

        private void HandleMove()
        {
            Vector2 moveInput = ReadVector2(moveAction);
            Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
            if (input.sqrMagnitude < 0.0001f)
            {
                return;
            }

            GetViewPlanarAxes(out Vector3 right, out Vector3 forward);

            float speed = moveSpeed;
            if (ReadButton(fastMoveAction))
            {
                speed *= Mathf.Max(1f, fastMoveMultiplier);
            }

            Vector3 moveDelta = (right * input.x + forward * input.z) * (speed * Time.deltaTime);
            transform.position += moveDelta;
        }

        private void HandleMouseDragPan()
        {
            if (!enableMouseDragPan || !ReadButton(panHoldAction))
            {
                return;
            }

            Vector2 lookDelta = ReadVector2(lookDeltaAction);
            GetViewPlanarAxes(out Vector3 right, out Vector3 forward);
            Vector3 panDelta = ((-right * lookDelta.x) + (-forward * lookDelta.y)) * panSpeed;
            transform.position += panDelta;
        }

        private void HandleZoom()
        {
            if (!enableZoom)
            {
                return;
            }

            Vector2 zoomInput = ReadVector2(zoomAction);
            float scroll = zoomInput.y * zoomInputMultiplier;
            if (Mathf.Abs(scroll) < 0.0001f)
            {
                return;
            }

            if (zoomCamera == null || !zoomCamera.orthographic)
            {
                return;
            }

            float targetSize = zoomCamera.orthographicSize - (scroll * zoomSpeed * Time.deltaTime);
            float minSize = Mathf.Min(minOrthographicSize, maxOrthographicSize);
            float maxSize = Mathf.Max(minOrthographicSize, maxOrthographicSize);
            zoomCamera.orthographicSize = Mathf.Clamp(targetSize, minSize, maxSize);
        }

        private void ClampToBounds()
        {
            Vector3 position = transform.position;

            if (useMovementBounds)
            {
                float minX = Mathf.Min(xBounds.x, xBounds.y);
                float maxX = Mathf.Max(xBounds.x, xBounds.y);
                float minZ = Mathf.Min(zBounds.x, zBounds.y);
                float maxZ = Mathf.Max(zBounds.x, zBounds.y);

                position.x = Mathf.Clamp(position.x, minX, maxX);
                position.z = Mathf.Clamp(position.z, minZ, maxZ);
            }

            transform.position = position;
        }

        private void GetViewPlanarAxes(out Vector3 right, out Vector3 forward)
        {
            Transform viewTransform = zoomCamera ? zoomCamera.transform : transform;

            forward = viewTransform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = Vector3.forward;
            }
            forward.Normalize();

            right = viewTransform.right;
            right.y = 0f;
            if (right.sqrMagnitude < 0.0001f)
            {
                right = Vector3.right;
            }
            right.Normalize();
        }

        private void EnableControls(bool enable)
        {
            if (enable)
            {
                moveAction.action.Enable();
                fastMoveAction.action.Enable();
                panHoldAction.action.Enable();
                lookDeltaAction.action.Enable();
                zoomAction.action.Enable();
            }
            else
            {
                moveAction.action.Disable();
                fastMoveAction.action.Disable();
                panHoldAction.action.Disable();
                lookDeltaAction.action.Disable();
                zoomAction.action.Disable();
            }
        }

        private static bool ReadButton(InputActionReference actionReference)
        {
            return actionReference?.action != null && actionReference.action.IsPressed();
        }

        private static Vector2 ReadVector2(InputActionReference actionReference)
        {
            return actionReference?.action?.ReadValue<Vector2>() ?? Vector2.zero;
        }
    }
}
