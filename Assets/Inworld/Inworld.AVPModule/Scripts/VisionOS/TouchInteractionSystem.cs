using System.Collections.Generic;
using Unity.PolySpatial.InputDevices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace LC.Interaction
{
    /// Singleton controller for the touch interaction system
    /// Manages snap targets and grabbables
    public class TouchInteractionSystem : MonoBehaviour
    {
        private static TouchInteractionSystem _instance;

        /// Singleton instance. Will create a new instance if accessed and one doesn't already exist in the scene
        public static TouchInteractionSystem Instance {
            get {
                if (_instance == null) _instance = FindObjectOfType<TouchInteractionSystem>();
                if (_instance == null) {
                    _instance = (new GameObject("TouchInteraction")).AddComponent<TouchInteractionSystem>();
                    _instance.OnTouchBegan = new UnityEvent<SpatialPointerState>();
                    _instance.OnTouchUpdate = new UnityEvent<SpatialPointerState>();
                    _instance.OnTouchEnd = new UnityEvent<SpatialPointerState>();
                }

                return _instance;
            }
        }

        public UnityEvent<SpatialPointerState> OnTouchBegan;
        /// Called each frame while a touch event is active
        public UnityEvent<SpatialPointerState> OnTouchUpdate;
        public UnityEvent<SpatialPointerState> OnTouchEnd;

        private List<MagneticSnapTarget> _targets;

        void Awake()
        {
            if (_targets == null) Instance._targets = new List<MagneticSnapTarget>();

            DontDestroyOnLoad(gameObject);
            EnhancedTouchSupport.Enable();
        }

        public void Update()
        {
            var touches = Touch.activeTouches;
            if (touches.Count == 0) return;
            foreach (var touch in touches) {
                var touchData = EnhancedSpatialPointerSupport.GetPointerState(touch);
                switch (touch.phase) {
                    case TouchPhase.Began:
                        OnTouchBegan?.Invoke(touchData);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        OnTouchUpdate?.Invoke(touchData);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        OnTouchEnd?.Invoke(touchData);
                        break;
                    case TouchPhase.None:
                    default:
                        break;
                }
            }
        }
        
        public static void RegisterSnapTarget(MagneticSnapTarget target)
        {
            if (Instance._targets == null) Instance._targets = new List<MagneticSnapTarget>();
            if (Instance._targets.Contains(target)) return;
            Instance._targets.Add(target);
        }

        public static void UnregisterSnapTarget(MagneticSnapTarget target)
        {
            if (Instance._targets == null) Instance._targets = new List<MagneticSnapTarget>();
            if (!Instance._targets.Contains(target)) return;
            Instance._targets.Remove(target);
        }

        /// Searches through all snap targets and finds the closest one to the target position.
        /// Returns null if there are none within range
        public static MagneticSnapTarget GetClosestTarget(Vector3 position)
        {
            var closestDistance = float.MaxValue;
            MagneticSnapTarget bestTarget = null;
            foreach (var target in Instance._targets) {
                if (target == null) continue;
                if (!target.MultipleObjects && target.SnappedObjects.Count > 0) continue;

                var sqrMag = (target.transform.position - position).sqrMagnitude;
                if (sqrMag > target.MagneticDistance * target.MagneticDistance) continue;

                if (sqrMag < closestDistance) {
                    closestDistance = sqrMag;
                    bestTarget = target;
                }
            }

            return bestTarget;
        }
    }
}