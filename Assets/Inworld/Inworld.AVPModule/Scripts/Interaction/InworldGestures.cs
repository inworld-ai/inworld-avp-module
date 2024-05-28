using Inworld;
using LC.DebugUtils;
using LC.InworldUtils;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace LC.Interaction
{
    public class InworldGestures : MonoBehaviour
    {
        public GestureRecognizerBase[] Gestures;
        public HandGestureDebugTexts DebugTexts;
        
        public void Awake()
        {
            foreach (var gesture in Gestures) {
                gesture.OnGestureStart.AddListener(HandleGesture);
            }
        }

        private void HandleGesture(GestureRecognizerBase gesture)
        {
            if (gesture is OneHandedGestureRecognizer oneHanded) {
                if (oneHanded.HandTrackingEvents.handedness == Handedness.Left)
                    DebugTexts.ReceiveGestureLeft(oneHanded.HandShapeOrPose.name);
                else if (oneHanded.HandTrackingEvents.handedness == Handedness.Right)
                    DebugTexts.ReceiveGestureRight(oneHanded.HandShapeOrPose.name);
            } else if (gesture is LoveHeartGesture) {
                DebugTexts.ReceiveGestureLeft("Love Heart");
            }

            //By sending this as a text, rather than a trigger, we avoid interrupting in-progress goal executions
            InworldController.CurrentCharacter.CancelResponse();
            InworldAsyncUtils.SendText($"*does a {gesture.PrettyName} gesture*");
        }
    }
}