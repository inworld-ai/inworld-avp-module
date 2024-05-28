using Unity.PolySpatial;
using UnityEngine;

namespace LC.Interaction
{
    public class ImmersiveModeObjectSwitcher : MonoBehaviour
    {
        public GameObject[] EnabledInBoundedMode;
        public GameObject[] EnabledInImmersiveMode;

        public void Awake()
        {
            VisionOsImmersiveMode.Instance.OnBeforeImmersiveModeChange.AddListener(mode =>
            {
                if (mode == VolumeCamera.PolySpatialVolumeCameraMode.Bounded) {
                    foreach(var obj in EnabledInImmersiveMode) obj.SetActive(false);
                } else {
                    foreach(var obj in EnabledInBoundedMode) obj.SetActive(false);
                }
            });
            VisionOsImmersiveMode.Instance.OnAfterImmersiveModeChange.AddListener(mode =>
            {
                if (mode == VolumeCamera.PolySpatialVolumeCameraMode.Bounded) {
                    foreach(var obj in EnabledInBoundedMode) obj.SetActive(true);
                } else {
                    foreach(var obj in EnabledInImmersiveMode) obj.SetActive(true);
                }
            });
        }

        public void Start()
        {
            foreach(var obj in EnabledInBoundedMode) obj.SetActive(!VisionOsImmersiveMode.IsImmersive);
            foreach(var obj in EnabledInImmersiveMode) obj.SetActive(VisionOsImmersiveMode.IsImmersive);
        }
    }
}