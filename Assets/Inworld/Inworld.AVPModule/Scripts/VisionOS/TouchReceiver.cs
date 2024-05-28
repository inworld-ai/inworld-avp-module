using System;
using Lunity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;

namespace LC.Interaction
{
    [Serializable] public class UnitySpatialPointerStateEvent : UnityEvent<SpatialPointerState> { }

    /// Base class for all objects that can receive VisionOS touch events
    [RequireComponent(typeof(Collider))]
    public class TouchReceiver : MonoBehaviour
    {
        public bool DirectTouchAllowed = false;
        
        [Header("Status")]
        [ReadOnly] public bool IsTouchDown;

        public UnitySpatialPointerStateEvent OnTouchDown;
        public UnitySpatialPointerStateEvent OnDrag;
        public UnityEvent OnTouchUp;

        public Transform TargetTransform;

        public Transform ThisTransform => TargetTransform == null ? transform : TargetTransform;

        protected SpatialPointerState _dataOnTouch;
        protected Vector3 _positionOnTouch;
        protected Quaternion _rotationOnTouch;
        
        protected Collider _collider;

        protected virtual void Awake()
        {
            _collider = GetComponent<Collider>();

            TouchInteractionSystem.Instance.OnTouchBegan.AddListener(data =>
            {
                if (data.targetObject != gameObject) return;
                if (!DirectTouchAllowed && data.Kind == SpatialPointerKind.Touch) return;
                _dataOnTouch = data;
                _positionOnTouch = ThisTransform.position;
                _rotationOnTouch = ThisTransform.rotation;
                IsTouchDown = true;
                OnTouchDown?.Invoke(_dataOnTouch);
            });
            TouchInteractionSystem.Instance.OnTouchUpdate.AddListener(data =>
            {
                if (data.targetObject != gameObject) return;
                if (!DirectTouchAllowed && data.Kind == SpatialPointerKind.Touch) return;
                IsTouchDown = true;
                OnDrag?.Invoke(data);
            });
            TouchInteractionSystem.Instance.OnTouchEnd.AddListener(data =>
            {
                if (data.targetObject != gameObject) return;
                if (!DirectTouchAllowed && data.Kind == SpatialPointerKind.Touch) return;
                IsTouchDown = false;
                OnTouchUp?.Invoke();
            });
        }
    }
}