using System.Collections;
using Inworld;
using Unity.PolySpatial;
using UnityEngine;

namespace LC.InworldUtils
{
    [RequireComponent(typeof(VolumeCamera))]
    public class AudioCaptureFocusManager : MonoBehaviour
    {
        private VolumeCamera _volume;
        private AudioCapture _audioCapture;

        public void Awake()
        {
            _volume = GetComponent<VolumeCamera>();
            _volume.OnWindowEvent.AddListener(HandleWindowEvent);
        }

        private void HandleWindowEvent(VolumeCamera.WindowState state)
        {
            if (_audioCapture == null) _audioCapture = FindObjectOfType<AudioCapture>();
            if (_audioCapture == null) return;
            
            //we wait 0.5 seconds before re-enabling audio to make extra sure that Unity actually has focus
            if (state.IsFocused) StartCoroutine(EnableAudio());
            else _audioCapture.enabled = false;
        }

        private IEnumerator EnableAudio()
        {
            yield return new WaitForSeconds(0.5f);
            _audioCapture.enabled = true;
        }
    }
}