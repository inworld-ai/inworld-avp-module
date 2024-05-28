using Lunity;
using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace LC.Interaction
{
    /// Utility class used to get player head/hand positions relative to the volume camera.
    /// When in unbounded MR mode on VisionOS, the best way to reposition the scene is to move the volume camera.
    /// However when we do this, the world-space position of the head/hands in Unity become offset by the inverse of the volume's transform.
    /// So whenever we need to access these transforms, we should do so via this class.
    public class VisionOsTransforms : SimpleSingleton<VisionOsTransforms>
    {
        [SerializeField] private VolumeCamera _volumeCamera;
        [SerializeField] private Transform _mainCamera;
        [SerializeField] private Transform _leftHand;
        [SerializeField] private Transform _rightHand;
        
        private Transform _volumeCameraTransform;
        private Transform VolumeCameraTransform {
            get {
                if (_volumeCameraTransform == null) _volumeCameraTransform = _volumeCamera.transform;
                return _volumeCameraTransform;
            }
        }

        public void Awake()
        {
            if (_mainCamera == null || _leftHand == null || _rightHand == null) {
                Debug.LogError("VisionOsTransforms doesn't have its transforms assigned!");
            }

            if (VolumeCameraTransform == null) {
                Debug.LogError("VisionOsTransforms doesn't have its volume camera assigned!");
            }
        }
        
        /// Gets the position of the player's head relative to the volume camera transform
        public static Vector3 GetCameraPosition(bool raw = false) =>
            raw ? Instance._mainCamera.position : Instance.VolumeCameraTransform.TransformPoint(Instance._mainCamera.position);
        /// Gets the rotation of the player's head relative to the volume camera transform
        public static Quaternion GetCameraRotation(bool raw = false) =>
            raw ? Instance._mainCamera.rotation : Instance.VolumeCameraTransform.rotation * Instance._mainCamera.rotation;

        /// Gets the position of the player's left hand relative to the volume camera transform
        public static Vector3 GetLeftHandPosition(bool raw = false) =>
            raw ? Instance._leftHand.position : Instance.VolumeCameraTransform.TransformPoint(Instance._leftHand.position);
        /// Gets the rotation of the player's right hand relative to the volume camera transform
        public static Quaternion GetLeftHandRotation(bool raw = false) =>
            raw ? Instance._leftHand.rotation : Instance.VolumeCameraTransform.rotation * Instance._leftHand.rotation;
        
        /// Gets the position of the player's left hand relative to the volume camera transform
        public static Vector3 GetRightHandPosition(bool raw = false) =>
            raw ? Instance._rightHand.position : Instance.VolumeCameraTransform.TransformPoint(Instance._rightHand.position);
        /// Gets the rotation of the player's right hand relative to the volume camera transform
        public static Quaternion GetRightHandRotation(bool raw = false) =>
            raw ? Instance._rightHand.rotation : Instance.VolumeCameraTransform.rotation * Instance._rightHand.rotation;

        /// Converts the given position into one relative to the volume camera transform
        public static Vector3 TransformPosition(Vector3 position) =>
            Instance.VolumeCameraTransform.TransformPoint(position);

        /// Converts the given Unity world-space rotation into a rotation relative to the volume camera transform
        public static Quaternion TransformRotation(Quaternion rotation) =>
            Instance.VolumeCameraTransform.rotation * rotation;

        /// Gets the position of the player's specified hand relative to the volume camera transform
        public static Vector3 GetHandPosition(Handedness hand)
        {
            switch (hand) {
                case Handedness.Left:
                    return GetLeftHandPosition();
                case Handedness.Right:
                    return GetRightHandPosition();
                default:
                    return Vector3.zero;
            }
        }
        /// Gets the rotation of the player's specified hand relative to the volume camera transform
        public static Quaternion GetHandRotation(Handedness hand)
        {
            switch (hand) {
                case Handedness.Left:
                    return GetLeftHandRotation();
                case Handedness.Right:
                    return GetRightHandRotation();
                default:
                    return Quaternion.identity;
            }
        }
    }
}