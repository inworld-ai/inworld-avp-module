using System.Collections;
using Inworld;
using LC.Interaction;
using Unity.PolySpatial;
using UnityEngine;

namespace LC.InworldUtils
{
    /// This is a cheeky workaround for a Polyspatial bug
    /// If we fail to explicitly initialize the microphone while in Immersive Mode and then switch to Bounded Mode,
    /// the audio engine will go haywire, with all AudioSources being played at 2x speed
    public class PulseAudioOnImmersiveMode : MonoBehaviour
    {
        public void Awake()
        {
            VisionOsImmersiveMode.Instance.OnAfterImmersiveModeChange.AddListener(PulseAudio);
        }

        private void PulseAudio(VolumeCamera.PolySpatialVolumeCameraMode state)
        {
            StopAllCoroutines();
            StartCoroutine(PulseAudioRoutine());
        }

        private IEnumerator PulseAudioRoutine()
        {
            var audioCapture = FindObjectOfType<AudioCapture>();
            if (audioCapture == null) yield break;
            Debug.Log("Pulsing audio");
            audioCapture.enabled = false;
            yield return new WaitForSeconds(0.1f);
            audioCapture.enabled = true;
        }
    }
}