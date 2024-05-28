using System.Collections;
using Inworld;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LC.UI
{
    /// Manages enabling/disabling sending microphone data to Inworld
    /// and manages the visuals of the record button
    public class RecordButtonRms : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Tooltip("How long we should keep recording active for after the user ends their touch event")]
        public float StopDelay = 0.5f;

        [Space(10f)]
        public float RmsRingMaxVolume = 0.1f;
        [Range(0.1f, 18f)] public float RingOpacityLerpSpeed = 12f;

        public HoldToTalkPrompt HoldToTalkPrompt;
        
        private CanvasGroup _rmsRing;

        private bool _recording;
        private Button _button;
        private AudioCapture _audioCapture;

        public void Awake()
        {
            _rmsRing = transform.Find("RmsRing").GetComponent<CanvasGroup>();
            _button = GetComponent<Button>();
            _rmsRing.gameObject.SetActive(false);
        }

        private void TryHookIntoAudioCapture()
        {
            if (_audioCapture != null) return;
            
            _audioCapture = FindObjectOfType<AudioCapture>();
            if (_audioCapture == null) return;
            
            //Control the opacity of the center image based on the live RMS volume coming in from the mic
            _audioCapture.OnAmplitude += rms =>
            {
                var clampedValue = Mathf.Clamp01(rms / RmsRingMaxVolume);
                _rmsRing.alpha = Mathf.Lerp(_rmsRing.alpha, clampedValue, Time.deltaTime * RingOpacityLerpSpeed);
            };
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _rmsRing.gameObject.SetActive(true);
            
            if (_recording) {
                //stop stopping the recording if they do pointer up then pointer down right away
                StopAllCoroutines();
                return;
            }

            //Innequin stops talking and starts listening!
            InworldController.CurrentCharacter.CancelResponse();
            InworldController.Instance.StartAudio();
            HoldToTalkPrompt.RegisterStart();
            _recording = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _rmsRing.gameObject.SetActive(false);
            HoldToTalkPrompt.RegisterStop();
            
            //delay stop for a moment to ensure we catch the end of the voice
            StartCoroutine(StopRecordingWithDelay());
        }

        private IEnumerator StopRecordingWithDelay()
        {
            yield return new WaitForSeconds(StopDelay);
            _recording = false;
            InworldController.Instance.PushAudio();
        }

        public void Update()
        {
            TryHookIntoAudioCapture();
            _button.interactable = _audioCapture?.IsRecording ?? false;
        }
    }
}