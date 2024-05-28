using UnityEngine;
using UnityEngine.XR.Hands;

namespace LC.Interaction
{
    /// Combines two gesture recognizers and a manual check to see that the thumb + index fingers are touching
    public class LoveHeartGesture : GestureRecognizerBase
    {
        public XRHandTrackingEvents LeftHandEvents;
        public XRHandTrackingEvents RightHandEvents;
        [Space(10f)]
        public OneHandedGestureRecognizer LeftHandGesture;
        public OneHandedGestureRecognizer RightHandGesture;
        [Tooltip("How far apart the index/thumb tips can be for them to be considered touching")]
        public float FingerTouchThresholdCm = 0.03f;

        private Vector3 _leftThumb;
        private Vector3 _leftIndex;
        private Vector3 _rightThumb;
        private Vector3 _rightIndex;
        
        public void Awake()
        {
            LeftHandEvents.jointsUpdated.AddListener(HandleLeftJoints);
            RightHandEvents.jointsUpdated.AddListener(HandleRightJoints);
        }

        private void HandleLeftJoints(XRHandJointsUpdatedEventArgs e)
        {
            var success = e.hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var thumbPose);
            if (!success) return;
            _leftThumb = thumbPose.position;
            success = e.hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var indexPose);
            if (!success) return;
            _leftIndex = indexPose.position;
        }
        
        private void HandleRightJoints(XRHandJointsUpdatedEventArgs e)
        {
            var success = e.hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var thumbPose);
            if (!success) return;
            _rightThumb = thumbPose.position;
            success = e.hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var indexPose);
            if (!success) return;
            _rightIndex = indexPose.position;
        }

        public override void Update()
        {
            _rawDetected = GetDetected();
            
            if (_rawDetected) {
                if (!_lastRawDetected) _detectedTime = Time.time;
            } else {
                if(_lastRawDetected) _undetectedTime = Time.time;
            }

            _lastRawDetected = _rawDetected;
            base.Update();
        }

        private bool GetDetected()
        {
            if (!LeftHandGesture.IsDetected) return false;
            if (!RightHandGesture.IsDetected) return false;
            
            var sqT = FingerTouchThresholdCm * FingerTouchThresholdCm;
            
            var thumbDist = (_leftThumb - _rightThumb).sqrMagnitude;
            if (thumbDist > sqT) return false;
            
            var indexDist = (_leftIndex - _rightIndex).sqrMagnitude;
            if (indexDist > sqT) return false;
            
            return true;
        }
    }
}