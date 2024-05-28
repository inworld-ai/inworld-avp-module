using LC.Interaction;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace LC
{
    /// This is the hat on Innequin's head within the individual scenes
    /// It can be pulled off to return to the MC scene
    public class RemovableHat : Grabbable
    {
        public GameObject RemoveHatTarget;
        private MagneticSnapTarget _defaultSnapTarget;
        private Transform _defaultParent;
        private bool _snappedToRemove = false;

        protected override void Awake()
        {
            base.Awake();
            _defaultSnapTarget = CurrentSnapTarget;
            _defaultParent = ThisTransform.parent;
        }

        protected override void Grab(SpatialPointerState touchData)
        {
            base.Grab(touchData);
            RemoveHatTarget.SetActive(true);
            ThisTransform.parent = null;
            _snappedToRemove = false;
        }

        protected override void Release()
        {
            base.Release();
            
            // if we release the hat away from any snap targets, return it to the original snap target on Innequin's head
            if (CurrentSnapTarget == null) {
                if (_defaultSnapTarget.MagnetizePosition) _targetPosition = _defaultSnapTarget.MagnetPosition;
                if (_defaultSnapTarget.MagnetizeRotation) _targetRotation = _defaultSnapTarget.MagnetRotation;
                _defaultSnapTarget.SnapObject(this);
            }

            _snappedToRemove = CurrentSnapTarget != _defaultSnapTarget;
            
            // either way, on release we hide the X target
            RemoveHatTarget.SetActive(false);
        }

        protected override void Update()
        {
            if (HoverEffect) HoverEffect.enabled = IsGrabbable;
            
            //if we're grabbed, moving towards the remove target, or in the progress of returning to Innequin's head,
            //behave as a normal snap target
            if (IsGrabbed || CurrentSnapTarget == null || _snappedToRemove || TargetSnapbackProgress < 0.999f) {
                base.Update();
                return;
            }
            
            //if we've reached Innequin's head, we reparent to the original transform to match the head movement exactly
            //(to avoid the hat clipping through by lagging one frame behind)
            ThisTransform.parent = _defaultParent;
            ThisTransform.localPosition = Vector3.zero;
            ThisTransform.localRotation = Quaternion.identity;
            
            _targetPosition = ThisTransform.position;
            _targetRotation = ThisTransform.rotation;
        }
    }
}