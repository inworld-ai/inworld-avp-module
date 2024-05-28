using LC.UI;
using UnityEngine;

namespace LC.Artefact
{
    /// Controls the behaviour and appearance of the artefact turntable
    /// Quite tightly-related to the hierarchy structure of the turntable, so see that (in the Artefact scene) for more details.
    public class Turntable : MonoBehaviour
    {
        public RotateRingAppearance RotateRing;
        
        [Header("Config")]
        public Vector2 RotationDegreesPerCm = new Vector2(18f, 3f);
        public float VerticalOffset = 0.04f;
        [Range(0f, 180f)] public float MaxRotationX = 30f;
        [Range(1f, 24f)] public float SmoothSpeed = 6f;
        
        private Transform _lifter;

        private bool _on;
        public float _startY;
        
        public float _targetAngleY;
        public float _targetAngleX;
        public float _angleX;
        public float _angleY;

        private Vector3 _defaultPosition;
        private Vector3 _activePosition;

        public void Awake()
        {
            _lifter = transform.parent;
            _defaultPosition = _lifter.localPosition;
            _activePosition = _defaultPosition + Vector3.up * VerticalOffset;
        }
        
        public void Update()
        {
            UpdateTargets();

            var lerp = Time.deltaTime * SmoothSpeed;
            _angleX = Mathf.Lerp(_angleX, _targetAngleX, lerp);
            _angleY = Mathf.Lerp(_angleY, _targetAngleY, lerp);
            
            transform.rotation = Quaternion.identity;
            transform.Rotate(_angleX, 0f, 0f, Space.World);
            transform.Rotate(0f, _angleY, 0f, Space.Self);

            _lifter.localPosition = Vector3.Lerp(_lifter.localPosition,
                RotateRing.IsOn ? _activePosition : _defaultPosition, lerp);
        }

        private void UpdateTargets()
        {
            if (!RotateRing.IsOn) {
                _targetAngleX = 0f;
                _on = false;
                return;
            } else if (!_on) {
                _angleY = transform.eulerAngles.y;
                _startY = transform.eulerAngles.y;
                _on = true;
            }

            _targetAngleX = Mathf.Clamp(RotationDegreesPerCm.y * RotateRing.TouchOffset.y * 100f, -MaxRotationX, MaxRotationX);
            _targetAngleY = _startY + RotationDegreesPerCm.x * RotateRing.TouchOffset.x * 100f;
        }
    }
}