using System.Collections.Generic;
using Lunity;
using UnityEngine;
using UnityEngine.Events;

namespace LC.Interaction
{
    /// Base class for snap targets that Grabbables can be placed onto
    public class MagneticSnapTarget : MonoBehaviour
    {
        [Tooltip("The distance from the object's origin that grabbable objects will be magnetically attached to")]
        public float MagneticDistance = 0.075f;
        [Tooltip("The strength of the magnetism effect")]
        [Range(1f, 8f)] public float MagnetStrength = 3f;
        [Tooltip("Offset from the object's origin that the magnetic attraction target position is set to")]
        public Vector3 PositionOffset = Vector3.zero;
        public Vector3 MagnetPosition => transform.TransformPoint(PositionOffset);
        public Quaternion MagnetRotation => transform.rotation;

        [Tooltip("Whether to apply magnetism to the grabbable's position")]
        public bool MagnetizePosition = true;
        [Tooltip("Whether to apply magnetism to the grabbable's rotation")]
        public bool MagnetizeRotation;
        [Tooltip("Whether multiple objects can be snapped to this snap target")]
        public bool MultipleObjects;

        [Tooltip("Objects currently snapped to this snap target")]
        [ReadOnly] public List<Grabbable> SnappedObjects;

        public UnityEvent<Grabbable> OnObjectSnap;
        public UnityEvent<Grabbable> OnObjectUnsnap;

        public virtual void Awake()
        {
            SnappedObjects = new List<Grabbable>();
        }

        public virtual void OnEnable()
        {
            TouchInteractionSystem.RegisterSnapTarget(this);
        }

        public virtual void OnDisable()
        {
            if (SnappedObjects.Count > 0) {
                for (var i = SnappedObjects.Count - 1; i >= 0; i--) {
                    UnsnapObject(SnappedObjects[i]);
                }
            }

            TouchInteractionSystem.UnregisterSnapTarget(this);
        }

        /// Applies magnetism to the input position and rotation
        public void Magnetize(Vector3 position, Quaternion rotation, out Vector3 newPosition,
            out Quaternion newRotation)
        {
            Magnetize(position, rotation, this, out newPosition, out newRotation);
        }

        /// Applies magnetism to the input position and rotation
        public static void Magnetize(Vector3 position, Quaternion rotation, MagneticSnapTarget snapTarget,
            out Vector3 newPosition, out Quaternion newRotation)
        {
            Magnetize(position, rotation,
                snapTarget.MagnetPosition, snapTarget.MagnetRotation,
                snapTarget.MagneticDistance, snapTarget.MagnetStrength,
                snapTarget.MagnetizePosition, snapTarget.MagnetizeRotation,
                out newPosition, out newRotation);
        }

        /// Applies magnetism to the input position and rotation
        public static void Magnetize(Vector3 position, Quaternion rotation,
            Vector3 magnetPosition, Quaternion magnetRotation,
            float magnetDistance, float magnetStrength,
            bool magnetizePosition, bool magnetizeRotation,
            out Vector3 newPosition, out Quaternion newRotation)
        {
            var deltaLength = (position - magnetPosition).magnitude;
            var magneticMovementAmount =
                0.1f + 0.9f * Mathf.Pow(Mathf.Clamp01(deltaLength / magnetDistance), magnetStrength);
            newPosition = magnetizePosition ? Vector3.Lerp(magnetPosition, position, magneticMovementAmount) : position;
            newRotation = magnetizeRotation
                ? Quaternion.Lerp(magnetRotation, rotation, magneticMovementAmount)
                : rotation;
        }

        /// Snaps the grabbable to this target - if it has room to accept a new grabbable
        /// Returns true if the grabbable was able to be snapped
        public virtual bool SnapObject(Grabbable grabbable, bool suppressEvent = false)
        {
            if (!MultipleObjects && SnappedObjects.Count > 0) return false;
            
            SnappedObjects.Add(grabbable);
            grabbable.CurrentSnapTarget = this;
            if (!suppressEvent) OnObjectSnap?.Invoke(grabbable);
            return true;
        }

        /// Removes the grabbable from this snap target
        public virtual void UnsnapObject(Grabbable grabbable, bool suppressEvent = false)
        {
            if (SnappedObjects.Contains(grabbable)) SnappedObjects.Remove(grabbable);
            grabbable.CurrentSnapTarget = null;
            if (!suppressEvent) OnObjectUnsnap?.Invoke(grabbable);
        }
    }
}