using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Movement
{
    public class Rotate : MonoBehaviour
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionReference lookDeltaAction;
        [SerializeField] private InputActionReference rotateHoldAction;
        [SerializeField] private Camera targetCamera;

        [Header("Mouse Rotation")]
        [SerializeField] private bool enableMouseRotation = true;
        [SerializeField] private float mouseYawSpeed = 120f;
        [SerializeField] private float mousePitchSpeed = 90f;

        [Header("Keyboard Rotation")]
        [SerializeField] private InputActionReference rotateLeftKey;
        [SerializeField] private InputActionReference rotateRightKey;
        [SerializeField] private float keyboardYawSpeed = 90f;

        [Header("Orbit Pivot")]
        [SerializeField] private float pivotHeight = 0f;
        [SerializeField] private float fallbackPivotDistance = 20f;

        [Header("Pitch Limits (Mouse Orbit)")]
        [SerializeField] private float minPitch = 20f;
        [SerializeField] private float maxPitch = 80f;

        private float _yaw;
        private float _pitch;

        private void OnEnable()
        {
            EnableAction(lookDeltaAction);
            EnableAction(rotateHoldAction);
            EnableAction(rotateLeftKey);
            EnableAction(rotateRightKey);
        }

        private void OnDisable()
        {
            DisableAction(lookDeltaAction);
            DisableAction(rotateHoldAction);
            DisableAction(rotateLeftKey);
            DisableAction(rotateRightKey);
        }

        private void Awake()
        {
           
            Vector3 euler = (targetCamera != null ? targetCamera.transform.rotation : transform.rotation).eulerAngles;
            _yaw = euler.y;
            _pitch = NormalizePitch(euler.x);
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
        }

        private void Update()
        {
            float yawDelta = GetMouseYawDelta() + GetKeyboardYawDelta();
            float pitchDelta = 0;//GetMousePitchDelta();

            if (Mathf.Abs(yawDelta) < 0.0001f && Mathf.Abs(pitchDelta) < 0.0001f)
            { 
                return;
            }
            
            OrbitAroundViewCenter(yawDelta, pitchDelta);
        }

        private float GetMouseYawDelta()
        {
            if (!enableMouseRotation || !ReadButton(rotateHoldAction))
            {
                return 0f;
            }

            Vector2 lookDelta = ReadVector2(lookDeltaAction);
            
            return lookDelta.x * mouseYawSpeed * Time.deltaTime;
        }

        private float GetMousePitchDelta()
        {
            if (!enableMouseRotation || !ReadButton(rotateHoldAction))
            {
                return 0f;
            }

            Vector2 lookDelta = ReadVector2(lookDeltaAction);

            float oldPitch = _pitch;
            _pitch = Mathf.Clamp(_pitch - (lookDelta.y * mousePitchSpeed * Time.deltaTime), minPitch, maxPitch);
            
            return _pitch - oldPitch;
        }

        private float GetKeyboardYawDelta()
        {
            float yawDirection = 0f;

            if (rotateLeftKey.action.IsPressed())
            {
                yawDirection -= 1f;
            }

            if (rotateRightKey.action.IsPressed())
            {
                yawDirection += 1f;
            }
            
            return yawDirection * keyboardYawSpeed * Time.deltaTime;
        }

        private void OrbitAroundViewCenter(float yawDelta, float pitchDelta)
        {
            if (!TryGetOrbitPivot(out Vector3 pivot))
            {
                return;
            }

            if (Mathf.Abs(yawDelta) > 0.0001f)
            {
                _yaw += yawDelta;
                transform.RotateAround(pivot, Vector3.up, yawDelta);
            }

            if (Mathf.Abs(pitchDelta) > 0.0001f)
            {
                Vector3 rightAxis = targetCamera ? targetCamera.transform.right : transform.right;
                transform.RotateAround(pivot, rightAxis, pitchDelta);
            }
        }

        private bool TryGetOrbitPivot(out Vector3 pivot)
        {
            Camera cam = targetCamera;
            if (!targetCamera)
            {
                pivot = default;
                return false;
            }

            Ray centerRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Plane pivotPlane = new Plane(Vector3.up, new Vector3(0f, pivotHeight, 0f));
            if (pivotPlane.Raycast(centerRay, out float enter))
            {
                pivot = centerRay.GetPoint(enter);
                return true;
            }

            pivot = cam.transform.position + cam.transform.forward * Mathf.Max(0.1f, fallbackPivotDistance);
            pivot.y = pivotHeight;
            return true;
        }

        private static float NormalizePitch(float rawPitch)
        {
            if (rawPitch > 180f)
            {
                rawPitch -= 360f;
            }

            return rawPitch;
        }
        
        private static void EnableAction(InputActionReference actionReference)
        {
            actionReference?.action?.Enable();
        }

        private static void DisableAction(InputActionReference actionReference)
        {
            actionReference?.action?.Disable();
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
