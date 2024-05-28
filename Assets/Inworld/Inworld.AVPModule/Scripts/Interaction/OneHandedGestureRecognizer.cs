using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

namespace LC.Interaction
{
    /// A modified version of Unity's StaticHandGesture component with a few extra features
    public class OneHandedGestureRecognizer : GestureRecognizerBase
    {
        [Tooltip("The hand tracking events component to subscribe to receive updated joint data to be used for gesture detection.")]
        public XRHandTrackingEvents HandTrackingEvents;

        [Tooltip("The hand shape or pose that must be detected for the gesture to be performed.")]
        public ScriptableObject HandShapeOrPose;
        
        [Tooltip("The target Transform to user for target conditions in the hand shape or pose.")]
        public Transform TargetTransform;

        [Tooltip("The interval at which the gesture detection is performed.")]
        public float GestureDetectionInterval = 0.1f;

        private XRHandShape _handShape;
        private XRHandPose _handPose;

        void OnEnable()
        {
            HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);

            _handShape = HandShapeOrPose as XRHandShape;
            _handPose = HandShapeOrPose as XRHandPose;
            if (_handPose != null && _handPose.relativeOrientation != null)
                _handPose.relativeOrientation.targetTransform = TargetTransform;
        }

        void OnDisable() => HandTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);

        void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
        {
            if (!isActiveAndEnabled || Time.timeSinceLevelLoad < _timeOfLastConditionCheck + GestureDetectionInterval) {
                return;
            }

            _rawDetected =
                HandTrackingEvents.handIsTracked &&
                _handShape != null && _handShape.CheckConditions(eventArgs) ||
                _handPose != null && _handPose.CheckConditions(eventArgs);

            if (_rawDetected) {
                if (!_lastRawDetected) _detectedTime = Time.time;
            } else {
                if(_lastRawDetected) _undetectedTime = Time.time;
            }

            _lastRawDetected = _rawDetected;
            _timeOfLastConditionCheck = Time.timeSinceLevelLoad;
        }
    }
}
