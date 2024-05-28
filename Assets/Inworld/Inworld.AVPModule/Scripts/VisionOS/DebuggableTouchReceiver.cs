using LC.Interaction;
using Lunity;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace LC.UI {
    
    /// This class isn't necessary, a base TouchReceiver would be fine
    /// This is just for testing in the editor
    public class DebuggableTouchReceiver : TouchReceiver
    {
        [Header("Debug")]
        public Vector3 DebugOffset;

        private Vector3 _touchPosition;
        private Vector3 _dragPosition;

        [EditorButton]
        public void DebugTouchDown()
        {
            _touchPosition = ThisTransform.position + Vector3.back;
            _dragPosition = _touchPosition;
            
            _dataOnTouch = new SpatialPointerState {
                interactionPosition = _touchPosition,
                inputDevicePosition = _touchPosition
            };
            OnTouchDown?.Invoke(_dataOnTouch);
        }

        [EditorButton]
        public void DebugDrag()
        {
            _dragPosition += DebugOffset;
            var data = new SpatialPointerState() {
                interactionPosition = _dragPosition,
                inputDevicePosition = _dragPosition,
                startInteractionPosition = _touchPosition,
            };
            OnDrag?.Invoke(data);
        }

        [EditorButton]
        private void DebugTouchUp()
        {
            OnTouchUp?.Invoke();
        }
    }
}