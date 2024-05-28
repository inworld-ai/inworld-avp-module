using Lunity;
using UnityEngine;
using UnityEngine.Events;

namespace LC.Interaction
{
    public class GestureRecognizerBase : MonoBehaviour
    {
        [System.Serializable] public class UnityGestureEvent : UnityEvent<GestureRecognizerBase> { }
        
        [Tooltip("Human-readable name of this gesture")]
        public string PrettyName;
        
        [Tooltip("The event fired when the gesture is performed.")]
        public UnityGestureEvent OnGestureStart;
        
        [Tooltip("The event fired when the gesture is ended.")]
        public UnityGestureEvent OnGestureEnd;
        
        [Tooltip("The minimum amount of time the hand must be held in the required shape and orientation for the gesture to be performed.")]
        public float MinimumHoldTime = 0.2f;
        
        [Tooltip("The minimum amount of the time the hand must not be in the required shape and orientation for the gesture to be considered ended (prevents flickering on/off)")]
        public float MinimumUnholdTime = 0.3f;
        
        [ReadOnly] public bool IsDetected;
        
        protected bool _lastRawDetected;
        protected bool _rawDetected;
        protected float _detectedTime;
        protected float _undetectedTime;
        
        protected float _timeOfLastConditionCheck;
        
        public virtual void Update()
        {
            if (_rawDetected && !IsDetected && Time.time - _detectedTime > MinimumHoldTime) {
                IsDetected = true;
                OnGestureStart?.Invoke(this);
            } else if (!_rawDetected && IsDetected && Time.time - _undetectedTime > MinimumUnholdTime) {
                IsDetected = false;
                OnGestureEnd?.Invoke(this);
            }
        }
    }
}