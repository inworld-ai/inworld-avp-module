using Lunity;
using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;

namespace LC.Interaction
{
    /// Base class for grabbable objects
    public class Grabbable : TouchReceiver
    {
        [Header("Status")]
        [ReadOnly] public bool IsGrabbed;
        public MagneticSnapTarget CurrentSnapTarget;
        
        [Header("Config")]
        [Tooltip("Whether this object should be able to be picked up")]
        public bool IsGrabbable = true;
        [Tooltip("Smoothing strength towards the target position - lower values = stronger smoothing")]
        [Range(0.1f, 48f)] public float SmoothingSpeed = 24f;
        [Tooltip("Whether the object should be restricted to the currently active Polyspatial volume camera")]
        public bool ClampPositionToVolume = true;
        [Tooltip("If true and the object is released, it will return to the snap target it was removed from if it isn't dropped onto a new snap target")]
        public bool MustBeSnapped;
        
        public UnityEvent OnGrab;
        public UnityEvent OnRelease;

        protected Vector3 _rawInteractionTargetPosition;
        protected Vector3 _targetPosition;
        protected Quaternion _targetRotation;
        private MagneticSnapTarget _lastSnapTarget;
        protected VisionOSHoverEffect HoverEffect;

        //when we have an active snap target, this value is animated from 0-1 to power the forced repositioning of this grabbable to match
        protected float TargetSnapbackProgress;
        private VolumeCamera _volume;

#if UNITY_EDITOR
        public Vector3 DebugOffset;
        private Vector3 _lastDebugPosition;
#endif

        protected Vector3 _currentVelocity;
        protected Vector3 _lastPosition;

        protected override void Awake()
        {
            base.Awake();

            HoverEffect = GetComponent<VisionOSHoverEffect>();
            
            _targetPosition = ThisTransform.position;
            _targetRotation = ThisTransform.rotation;
            _volume = FindObjectOfType<VolumeCamera>();

            if (CurrentSnapTarget != null) {
                //snapped by default
                if (CurrentSnapTarget.SnapObject(this, true)) {
                    _lastSnapTarget = CurrentSnapTarget;
                    TargetSnapbackProgress = 1f;   
                } else {
                    CurrentSnapTarget = null;
                }
            }
            
            if(CurrentSnapTarget == null && MustBeSnapped) {
                Debug.LogWarning("Grabbable has MustBeSnapped enabled, but no starting snap target. MustBeSnapped will have no effect until the object is placed in a snap target");
            }

            OnTouchDown.AddListener(Grab);
            OnDrag.AddListener(HandleUpdate);
            OnTouchUp.AddListener(Release);
        }

        protected virtual void Update()
        {
            if (HoverEffect) HoverEffect.enabled = IsGrabbable;
            
            _lastPosition = ThisTransform.position;
            
            //if we're snapped, don't use smoothing, just lerp to the actual position over 0.5 seconds and then stay there
            if (CurrentSnapTarget != null) MoveTowardsSnapTarget();
            //Move towards the new position + rotation, and calculate velocity
            else MoveTowardsTargets();
        }

        protected virtual void MoveTowardsSnapTarget()
        {
            TargetSnapbackProgress = Mathf.Clamp01(TargetSnapbackProgress + Time.deltaTime / 0.5f);
            if (CurrentSnapTarget.MagnetizePosition) {
                ThisTransform.position = Vector3.Lerp(ThisTransform.position, CurrentSnapTarget.MagnetPosition, TargetSnapbackProgress);
                _targetPosition = ThisTransform.position;
            } else {
                ThisTransform.position =
                    Vector3.Lerp(ThisTransform.position, _targetPosition, Time.deltaTime * SmoothingSpeed);
            }

            if (CurrentSnapTarget.MagnetizeRotation) {
                ThisTransform.rotation =
                    Quaternion.Lerp(ThisTransform.rotation, CurrentSnapTarget.transform.rotation, TargetSnapbackProgress);
                _targetRotation = ThisTransform.rotation;
            } else {
                ThisTransform.rotation =
                    Quaternion.Lerp(ThisTransform.rotation, _targetRotation, Time.deltaTime * SmoothingSpeed);
            }
            _currentVelocity = transform.InverseTransformDirection(ThisTransform.position - _lastPosition) /
                               Time.deltaTime;
        }

        protected virtual void MoveTowardsTargets()
        {
            ThisTransform.position = Vector3.Lerp(ThisTransform.position, _targetPosition, Time.deltaTime * SmoothingSpeed);
            ThisTransform.rotation = Quaternion.Lerp(ThisTransform.rotation, _targetRotation, Time.deltaTime * SmoothingSpeed);
            _currentVelocity = transform.InverseTransformDirection(ThisTransform.position - _lastPosition) /
                               Time.deltaTime;
        }

        protected virtual void Grab(SpatialPointerState touchData)
        {
            if (!IsGrabbable) return;
            if (IsGrabbed) return;

            Debug.Log("Grabbed " + gameObject.name);
            _rawInteractionTargetPosition = transform.position;
            if (CurrentSnapTarget != null) {
                TargetSnapbackProgress = 0f;
                CurrentSnapTarget.UnsnapObject(this);
            }

            IsGrabbed = true;
            OnGrab?.Invoke();
        }

        protected virtual void HandleUpdate(SpatialPointerState touchData)
        {
            if (!IsGrabbed) return;
            
            var delta = touchData.interactionPosition - touchData.startInteractionPosition;
            var rawNewPosition = _positionOnTouch + delta;
            
            //If we're within range of a snap target, we apply a 'magnetism' force to the position to provide subtle feedback to the user
            var snapTarget = TouchInteractionSystem.GetClosestTarget(rawNewPosition);
            if (snapTarget != null)
                snapTarget.Magnetize(rawNewPosition, _targetRotation, out rawNewPosition, out _targetRotation);
            _targetPosition = rawNewPosition;
            _rawInteractionTargetPosition = _targetPosition;
            
            if (!ClampPositionToVolume) return;
            
            //If we have ClampPositionToVolume enabled, ensure the new position is within the volume bounds
            //todo: take object bounding box into account to avoid cutting it off. For now we're just insetting the borders by 5cm.
            if (_volume == null || _volume.WindowMode == VolumeCamera.PolySpatialVolumeCameraMode.Unbounded) return;
            var min = _volume.transform.position + new Vector3(-_volume.Dimensions.x * 0.5f + 0.05f,
                -_volume.Dimensions.y * 0.5f + 0.025f,
                -_volume.Dimensions.z * 0.5f + 0.05f);
            var max = _volume.transform.position + new Vector3(_volume.Dimensions.x * 0.5f - 0.05f,
                _volume.Dimensions.y * 0.5f - 0.05f,
                _volume.Dimensions.z * 0.5f - 0.05f);
            
            _targetPosition.x = Mathf.Min(max.x, Mathf.Max(min.x, _targetPosition.x));
            _targetPosition.y = Mathf.Min(max.y, Mathf.Max(min.y, _targetPosition.y));
            _targetPosition.z = Mathf.Min(max.z, Mathf.Max(min.z, _targetPosition.z));
            _rawInteractionTargetPosition = _targetPosition;
        }

        protected virtual void Release()
        {
            if (!IsGrabbed) return;
            
            Debug.Log("Released " + gameObject.name);
            
            //Look for the nearest snap target
            var snapTarget = TouchInteractionSystem.GetClosestTarget(_targetPosition);
            
            //If we didn't find one and MustBeSnapped is enabled, snap back to it (i.e. the target that we started at during this grab interaction)
            if (snapTarget == null && MustBeSnapped && _lastSnapTarget != null) {
                snapTarget = _lastSnapTarget;
            }

            //If we did find one, snap to it!
            if (snapTarget != null) {
                if (!SnapToTarget(snapTarget)) _targetRotation = _rotationOnTouch;
            } else {
                _targetRotation = _rotationOnTouch;
            }

            IsGrabbed = false;
            OnRelease?.Invoke();
        }

        public virtual bool SnapToTarget(MagneticSnapTarget target)
        {
            if (target.SnapObject(this)) {
                if (target.MagnetizePosition) _targetPosition = target.MagnetPosition;
                if (target.MagnetizeRotation) _targetRotation = target.MagnetRotation;
                _lastSnapTarget = target;
                return true;
            }

            return false;
        }

        protected virtual void OnDestroy()
        {
            if (CurrentSnapTarget != null) {
                CurrentSnapTarget.UnsnapObject(this);
            }
        }

#if UNITY_EDITOR
        //These methods allow us to do some basic interaction testing from the editor
        //without access to proper pointer states
        
        [EditorButton]
        public void DebugGrab()
        {
            Grab(new SpatialPointerState());
        }

        [EditorButton]
        public void ApplyDebugOffset()
        {
            //_targetPosition += DebugOffset;
            _lastDebugPosition += DebugOffset;
            HandleUpdate(new SpatialPointerState {
                interactionPosition = _lastDebugPosition,
                startInteractionPosition = Vector3.zero,
            });
        }

        [EditorButton]
        public void DebugRelease()
        {
            Release();
            _lastDebugPosition = Vector3.zero;
        }
#endif
    }
}