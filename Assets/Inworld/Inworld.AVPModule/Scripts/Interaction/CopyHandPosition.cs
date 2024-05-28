using UnityEngine;
using UnityEngine.XR.Hands;

namespace LC.Interaction
{
    public class CopyHandPosition : MonoBehaviour
    {
        public Handedness Hand = Handedness.Left;

        public void Awake()
        {
            var handEvents = FindObjectsOfType<XRHandTrackingEvents>();
            foreach (var he in handEvents) {
                if (he.handedness != Hand) continue;
                he.poseUpdated.AddListener(pose =>
                {
                    transform.position = pose.position;
                    transform.rotation = pose.rotation;
                });
            }
        }
    }
}