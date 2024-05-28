using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace LC.Interaction
{
    /// Similar to the default grabbable, but is effectively magnetically attached to whatever position it starts at,
    /// even if not connected to a MagneticSnapTarget
    public class MagneticGrabbable : Grabbable
    {
        public float MagneticDistance = 0.075f;
        [Range(1f, 8f)] public float MagnetStrength = 3f;

        private Vector3 _magneticPosition;

        protected override void Awake()
        {
            base.Awake();
            _magneticPosition = ThisTransform.position;
        }

        protected override void HandleUpdate(SpatialPointerState touchData)
        {
            base.HandleUpdate(touchData);
            MagneticSnapTarget.Magnetize(
                _targetPosition, Quaternion.identity,
                _magneticPosition, Quaternion.identity,
                MagneticDistance, MagnetStrength,
                true, false,
                out _targetPosition, out _);
        }

        protected override void Release()
        {
            //snap back if it hasn't been dragged far enough
            var delta = ThisTransform.position - _positionOnTouch;
            var deltaLength = delta.magnitude;
            if (deltaLength < MagneticDistance) _targetPosition = _magneticPosition;
        }
    }
}