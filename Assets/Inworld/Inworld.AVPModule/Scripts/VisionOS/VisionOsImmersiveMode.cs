using System;
using System.Collections;
using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using VolumeMode = Unity.PolySpatial.VolumeCamera.PolySpatialVolumeCameraMode;

namespace LC.Interaction
{
    /// Utility class to manage the toggling between bounded and unbounded mixed-reality modes in Polyspatial
    public class VisionOsImmersiveMode : MonoBehaviour
    {
        [Serializable]
        public class VolumeCameraModeEvent : UnityEvent<VolumeMode> { }

        private static VisionOsImmersiveMode _instance;
        public static VisionOsImmersiveMode Instance {
            get {
                if (_instance == null) _instance = FindObjectOfType<VisionOsImmersiveMode>();
                if (_instance == null) throw new UnityException("No Immersive Mode controller found in scene");
                return _instance;
            }
        }

        [Header("Configuration")]
        [Tooltip(
            "When switching to unbounded mode, the volume origin will be offset by this vector relative to the camera position")]
        public Vector3 UnboundedVolumeStartOffset = new Vector3(0f, -0.05f, 1f);
        [Tooltip("The volume camera config ScriptableObject for bounded mode. Must be in Resources")]
        public VolumeCameraWindowConfiguration BoundedConfig;
        [Tooltip("The volume camera config ScriptableObject for unbounded mode. Must be in Resources")]
        public VolumeCameraWindowConfiguration UnboundedConfig;

        [Header("Status")]
        [Tooltip(
            "When switching modes, this is set immediately - but VisionOS might take a moment to actually change the mode of our application")]
        public VolumeMode TargetMode;

        [Tooltip("This is only updated once the volume mode of our application has actually changed")]
        public VolumeMode CurrentMode;

        /// Whether we are currently in unbounded mode or not
        public static bool IsImmersive => Instance.CurrentMode == VolumeMode.Unbounded;

        [Header("Events")]
        public VolumeCameraModeEvent OnBeforeImmersiveModeChange;
        public VolumeCameraModeEvent OnAfterImmersiveModeChange;
        public UnityEvent OnRecenterStart;
        public UnityEvent<Vector3> OnRecenter;

        private VolumeCamera _volume;
        private Transform _cameraTransform;
        private bool _switching;
        private bool _initialized;

        public void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_initialized) return;

            _volume = FindObjectOfType<VolumeCamera>();
            TargetMode = _volume.WindowMode;
            CurrentMode = TargetMode;
            _cameraTransform = Camera.main.transform;

            _initialized = true;
        }

        /// Sets the mode to Unbounded if we're currently in Bounded mode (and vice-versa)
        /// Note that VisionOS may take a few moments to actually perform the change
        public void ToggleMode()
        {
            ApplyMode(CurrentMode == VolumeMode.Bounded ? VolumeMode.Unbounded : VolumeMode.Bounded);
        }

        /// Sets the mode to the specified mode
        /// Note that VisionOS may take a few moments to actually perform the change
        public void ApplyMode(VolumeMode mode)
        {
            Initialize();
            if (_switching) return;
            StartCoroutine(SwitchModeRoutine(mode));
        }

        private IEnumerator SwitchModeRoutine(VolumeMode mode)
        {
            TargetMode = mode;
            OnBeforeImmersiveModeChange?.Invoke(mode);
            _switching = true;
            Debug.Log("Moving into " + mode + " mode");

            _volume.WindowConfiguration = mode == VolumeMode.Bounded ? BoundedConfig : UnboundedConfig;
            if (mode == VolumeMode.Unbounded) {
                //we need to wait for VisionOS to actually switch the mode
                //the events coming out of Unity are unreliable, so instead we wait for the camera to move
                var startCameraPos = _cameraTransform.position;
                var cameraOffset = 0f;
                
#if UNITY_EDITOR
                //camera doesn't move in the editor, of course, so we add some random movement just for testing
                _cameraTransform.position = Random.insideUnitSphere * 0.2f;
                _cameraTransform.Rotate(0f, -30f + Random.value * 60f, 0f);
#endif
                while (cameraOffset < 0.01f) {
                    cameraOffset = (startCameraPos - _cameraTransform.position).sqrMagnitude;
                    //Debug.Log(cameraOffset);
                    yield return null;
                }

                Debug.Log("Camera has moved sufficiently, waiting 0.5 extra seconds");
                yield return new WaitForSeconds(0.5f);
            }

            //now we actually update the volume camera to ensure the volume's content appears in the correct spot
            //reset volume camera transform
            CurrentMode = mode;
            ResetPosition();

            OnAfterImmersiveModeChange?.Invoke(mode);
            _switching = false;
        }

        public void ResetPosition()
        {
            _volume.transform.position = Vector3.zero;
            _volume.transform.rotation = Quaternion.identity;
            if (CurrentMode == VolumeMode.Unbounded) {
                StartCoroutine(ResetPositionRoutine());
            }
        }

        private IEnumerator ResetPositionRoutine()
        {
            OnRecenterStart?.Invoke();
            yield return new WaitForSeconds(0.5f);
            
            //places the volume on the camera
            _volume.transform.position = -_cameraTransform.position;
                
            //has the volume face the camera
            var offsetForward = _cameraTransform.forward;
            offsetForward.y = 0f;
            offsetForward.Normalize();
            _volume.transform.rotation = Quaternion.LookRotation(offsetForward, Vector3.up);
                
            //apply preconfigured offset from the camera
            var offsetRight = Vector3.Cross(offsetForward, Vector3.up);
            var offset = offsetRight * UnboundedVolumeStartOffset.x
                         + Vector3.up * UnboundedVolumeStartOffset.y
                         + offsetForward * UnboundedVolumeStartOffset.z;
            _volume.transform.Translate(-offset, Space.World);

            OnRecenter?.Invoke(_volume.transform.position);
        }
    }
}