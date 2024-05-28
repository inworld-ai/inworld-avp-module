using UnityEngine;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Hands.Samples.Gestures.DebugTools;

namespace LC.DebugUtils
{
    public class GestureDebugUiShapeAssigner : MonoBehaviour
    {
        public XRHandShape[] Shapes;
        public XRHandShapeDebugUI DebugUI;
        
        public void SetShape(int index)
        {
            if (index < 0 || index >= Shapes.Length) return;
            DebugUI.handShapeOrPose = Shapes[index];
        }
    }
}