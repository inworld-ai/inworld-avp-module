using System.Collections;
using System.Collections.Generic;
using LC.UI;
using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace LC.Interaction
{
    [RequireComponent(typeof(BoxCollider))]
    public class VolumeGrabHandle : DebuggableTouchReceiver
    {
        [Header("Volume Positioning Config")]
        public float MinAngularSizeDegrees = 5f;

        [Space(10f)]
        [Tooltip("When the hand has moved by Reference Input Offset cm, the volume will move Refernece Output Offset m in the world-space direction")]
        public float ReferenceInputOffsetCm = 30f;
        [Tooltip("When the hand has moved by Reference Input Offset cm, the volume will move Refernece Output Offset m in the world-space direction")]
        public float ReferenceOutputOffsetM = 3f;
        public float MovementExponent = 5f;
        [Space(10f)]
        public float SmoothingSpeed = 8f;
        public float MaxDistanceFromCamera = 10f;

        private VolumeCamera _volume;
        private bool _isGrabbed;
        private bool _everGrabbed;
        private bool _firstFrame;
        private BoxCollider _boxCollider;
        private Vector3 _defaultColliderSize;

        private Vector3 _targetPosition;
        private Vector3 _startPosition;
        private Vector3 _startHandPosition;
        private bool _recentering;
        
        protected override void Awake()
        {
            base.Awake();
            
            _volume = FindObjectOfType<VolumeCamera>();
            _boxCollider = (BoxCollider)_collider;
            _defaultColliderSize = _boxCollider.size;

            OnTouchDown.AddListener(StartDragging);
            OnDrag.AddListener(HandleDrag);
            OnTouchUp.AddListener(StopDragging);
            
            gameObject.SetActive(false);
        }

        public void Start()
        {
            VisionOsImmersiveMode.Instance.OnRecenterStart.AddListener(() =>
            {
                Debug.Log("Recenter Start");
                _recentering = true;
            });
            VisionOsImmersiveMode.Instance.OnRecenter.AddListener(newPos =>
            {
                Debug.Log("Recenter End: " + newPos);
                _targetPosition = newPos;
                _recentering = false;
            });
        }

        private void StartDragging(SpatialPointerState pointerState)
        {
            _isGrabbed = true;
            _firstFrame = true;
            _everGrabbed = true;
        }

        private void HandleDrag(SpatialPointerState pointerState)
        {
            if (!_isGrabbed) return;
            if (_firstFrame) {
                _startPosition = _volume.transform.position;
                _startHandPosition = _volume.transform.InverseTransformPoint(pointerState.inputDevicePosition);
                _targetPosition = _startPosition;
                _firstFrame = false;
                return;
            }
            
            var rawOffset = _volume.transform.InverseTransformPoint(pointerState.inputDevicePosition) - _startHandPosition;
            var offsetMag = rawOffset.magnitude;
            var offsetDir = rawOffset / offsetMag;
            var scaledOffsetMag = GetMovementAmount(offsetMag);
            var scaledOffset = offsetDir * scaledOffsetMag;
            if (offsetMag <= 0f) scaledOffset = Vector3.zero;
            _targetPosition = _startPosition - scaledOffset;
            _targetPosition = ClampPosition(_targetPosition);
        }

        private void StopDragging()
        {
            _isGrabbed = false;
        }

        public void Update()
        {
            if (!_everGrabbed) return;
            if (_recentering) return;
            
            var lerp = Time.deltaTime * SmoothingSpeed;
            _volume.transform.position = Vector3.Lerp(_volume.transform.position, _targetPosition, lerp);

            //increase the size of the collider to maintain a constant angular size
            var camPos = VisionOsTransforms.GetCameraPosition(true);
            var width = Mathf.Max(_defaultColliderSize.x, 2f * (camPos - _volume.transform.position).magnitude * Mathf.Tan(MinAngularSizeDegrees));
            var colliderScale = width / _defaultColliderSize.x;
            var newSize = _defaultColliderSize * colliderScale;
            _boxCollider.size = newSize;
        }

        private float GetMovementAmount(float inputAmount)
        {
            inputAmount = Mathf.Clamp(inputAmount, -ReferenceInputOffsetCm * 0.01f, ReferenceInputOffsetCm * 0.01f);
            var wasNegative = inputAmount < 0f;
            if (wasNegative) inputAmount *= -1f;
            var value = ReferenceOutputOffsetM * Mathf.Pow(ReferenceInputOffsetCm, -MovementExponent) * Mathf.Pow(inputAmount * 100f, MovementExponent);
            if (wasNegative) value *= -1f;
            return value;
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            var camPos = VisionOsTransforms.GetCameraPosition(true);
            var cameraOffset = position - camPos;
            if (cameraOffset.sqrMagnitude <= MaxDistanceFromCamera * MaxDistanceFromCamera) return position;
            var dir = cameraOffset.normalized;
            return camPos + dir * MaxDistanceFromCamera;
        }
    }
}