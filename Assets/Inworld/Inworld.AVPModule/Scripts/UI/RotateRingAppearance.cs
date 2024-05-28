using LC.Interaction;
using Lunity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;

namespace LC.UI {
    
    /// Controls the appearance and animation of the rotation handle
    public class RotateRingAppearance : MonoBehaviour
    {
        public TouchReceiver Handle;
        
        [Header("Appearance")]
        [Range(1f, 24f)] public float SmoothingSpeed = 12f;
        public float VerticalOffset = 0.02f;
        public float MaxRingTiltDegrees = 15f;
        public float RotationDegreesPerCm = 9f;
        public float VerticalOffsetPerCm = 0.25f;
        public float ActiveScale = 0.72f;

        private float _defaultScale;

        [Header("Status")]
        [ReadOnly] public bool IsOn;
        [ReadOnly] public Vector2 TouchOffset; 
        
        public UnityEvent<Vector2> OnGetOffset;
        
        private float _targetHeight = 0f;
        private float _targetRotation = 0f;
        private float _targetTilt = 0f;
        private float _targetScale = 0f;

        private Transform _ring; 
        private Transform _rotator;
        private MeshFlipbook _handleFlipbook;

        private Vector3 _touchFwd;
        private Vector3 _touchRight;
        
        protected void Awake()
        {
            _ring = transform.Find("SolidRing");
            _rotator = transform.Find("Rotator");
            _handleFlipbook = Handle.GetComponent<MeshFlipbook>();
            
            Handle.OnTouchDown.AddListener(TurnOn);
            Handle.OnDrag.AddListener(HandleDrag);
            Handle.OnTouchUp.AddListener(TurnOff);

            _defaultScale = transform.localScale.x;
            _targetScale = _defaultScale;
            
            TurnOff();
        }

        public void TurnOn(SpatialPointerState touchData)
        {
            _ring.gameObject.SetActive(true);
            _targetHeight = VerticalOffset;
            _handleFlipbook.TargetPosition = 1f;
            
            _touchFwd = touchData.interactionPosition - transform.position;
            _touchFwd.y = 0f;
            _touchFwd.Normalize();

            _touchRight = Vector3.Cross(_touchFwd, Vector3.up);

            _targetScale = ActiveScale;

            TouchOffset = Vector2.zero;

            IsOn = true;
        }

        public void HandleDrag(SpatialPointerState data)
        {
            var offset = data.interactionPosition - data.startInteractionPosition;
            var rightAmount = Vector3.Dot(offset, _touchRight);
            var upAmount = offset.y;

            TouchOffset = new Vector2(rightAmount, upAmount);
            OnGetOffset?.Invoke(TouchOffset);

            _targetRotation = RotationDegreesPerCm * rightAmount * 100f;
            _targetTilt = Mathf.Clamp(
                VerticalOffsetPerCm * upAmount * 100f,
                -MaxRingTiltDegrees,
                MaxRingTiltDegrees
            );
        }

        public void TurnOff()
        {
            _ring.gameObject.SetActive(false);
            _handleFlipbook.TargetPosition = 0f;
            _targetHeight = 0f;
            _targetRotation = 0f;
            _targetTilt = 0f;
            _targetScale = _defaultScale;
            IsOn = false;
        }

        public void Update()
        {
            var lerp = Time.deltaTime * SmoothingSpeed;
            var t = transform;

            var newPos = Vector3.back * _targetHeight / t.lossyScale.y;
            transform.localPosition = Vector3.Lerp(t.localPosition, newPos, lerp);

            var newX = Mathf.LerpAngle(t.localEulerAngles.x, _targetTilt, lerp);
            transform.localEulerAngles = Vector3.right * newX;
            
            transform.localScale = Vector3.Lerp(t.localScale, Vector3.one * _targetScale, lerp);
            
            var newZ = Mathf.LerpAngle(_rotator.localEulerAngles.z, _targetRotation, lerp);
            _rotator.localEulerAngles = new Vector3(0f, 0f, newZ);
        }
    }
}