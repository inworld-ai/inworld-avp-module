using System;
using System.Collections;
using UnityEngine;

namespace LC.Interaction
{
    /// Ensures that we are able to access the microphone on VisionOS
    /// For an unknown reason, sometimes Polyspatial just fails to start the microphone in on-device builds
    /// Bizarrely, the microphone will successfully initialize after a variable amount of time.
    /// Sometimes half a second, sometimes as much as 10 seconds or more!
    /// This class provides a coroutine which will keep retrying to start the mic and provide a callback when it succeeds / fails
    public class VisionOsMicEnabler : MonoBehaviour
    {
        public enum MicAccessCallbackResult
        {
            Success = 0,
            NoDevicesAvailable = 1,
            FailedToStartMicrophone = 2,
            NoAudioDetected = 3,
        }
        
        /// Try to open the microphone to ensure we have access
        /// onResult will return Success/0 if we can successfully begin recording. Positive integer enum results
        /// indicate failure + reason
        public static Coroutine GetMicAccess(Action<MicAccessCallbackResult> onResult, int attempts = 100)
        {
            var runnerObj = new GameObject("VisionOS Microphone Enabler");
            var instance = runnerObj.AddComponent<VisionOsMicEnabler>();
            return instance.StartCoroutine(instance.GetMicAccessInternal(onResult, attempts));
        }

        private IEnumerator GetMicAccessInternal(Action<MicAccessCallbackResult> onResult, int attempts)
        {
            // No microphone devices available
            if (Microphone.devices.Length == 0) {
                onResult?.Invoke(MicAccessCallbackResult.NoDevicesAvailable);
                StartCoroutine(DestroySelf());
                yield break;
            }
            var deviceName = Microphone.devices[0];
            var data = new float[1];
            AudioClip clip = null;
            var attempt = 0;
            
            // Try to start the microphone
            while (true) {
                if (attempt >= attempts) break;
                attempt++;
                
                clip = Microphone.Start(deviceName, true, 1, 16000);
                if (!Microphone.IsRecording(deviceName)) {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }
                break;
            }
            
            // Failed to start the mic?
            if (!clip || attempt >= attempts) {
                if (clip) Destroy(clip);
                Microphone.End(deviceName);
                onResult(MicAccessCallbackResult.FailedToStartMicrophone);
                StartCoroutine(DestroySelf());
                yield break;
            }

            // Try to get audio data out of the microphone
            attempt = 0;
            while(true) {
                if (attempt >= attempts) break;
                attempt++;
                
                clip.GetData(data, 0);
                if (Mathf.Abs(data[0]) > Mathf.Epsilon) {
                    onResult?.Invoke(MicAccessCallbackResult.Success);
                    Microphone.End(deviceName);
                    Destroy(clip);
                    StartCoroutine(DestroySelf());
                    yield break;
                }
            }

            // Failed to start/get audio from the microphone
            if (clip) Destroy(clip);
            Microphone.End(deviceName);
            onResult?.Invoke(MicAccessCallbackResult.NoAudioDetected);
            StartCoroutine(DestroySelf());
        }

        // Waits a frame, then destroys the dummy GameObject created to run the main mic-check coroutine
        private IEnumerator DestroySelf()
        {
            yield return null;
            Destroy(gameObject);
        }
    }
}