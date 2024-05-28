using System.Collections;
using System.Collections.Generic;
using LC.Interaction;
using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Hands;

namespace LC
{
    public class ImmersiveModeHandUi : MonoBehaviour
    {
        public XRHandTrackingEvents HandEvents;
        public XRHandJointID TargetJoint = XRHandJointID.Palm;
        public Button RecenterButton;

        private GameObject _visibilityParent;

        public void Awake()
        {
            _visibilityParent = transform.GetChild(0).gameObject;
            _visibilityParent.SetActive(false);
            HandEvents.jointsUpdated.AddListener(HandleJoints);
            RecenterButton.onClick.AddListener(TriggerRecenter);
        }

        public void Start()
        {
            VisionOsImmersiveMode.Instance.OnAfterImmersiveModeChange.AddListener(mode =>
            {
                _visibilityParent.SetActive(mode == VolumeCamera.PolySpatialVolumeCameraMode.Unbounded);
            });
        }
        
        private void HandleJoints(XRHandJointsUpdatedEventArgs e)
        {
            if (!VisionOsImmersiveMode.IsImmersive) return;
            
            var success = e.hand.GetJoint(TargetJoint).TryGetPose(out var pose);
            if (!success) return;
            transform.position = VisionOsTransforms.TransformPosition(pose.position);
            transform.rotation = VisionOsTransforms.TransformRotation(pose.rotation);
        }

        public void TriggerRecenter()
        {
            VisionOsImmersiveMode.Instance.ResetPosition();
        }
    }
}