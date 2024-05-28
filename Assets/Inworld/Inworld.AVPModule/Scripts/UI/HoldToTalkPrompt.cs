using System.Collections;
using Lunity;
using UnityEngine;

namespace LC.UI
{
    public class HoldToTalkPrompt : MonoBehaviour
    {

        [Tooltip("If the push-to-talk button is held for less than this duration, the prompt will trigger")]
        public float ThresholdDuration = 0.5f;
        [Tooltip("The minimum interval between prompts")]
        public float Cooldown = 5f;
        [Tooltip("How long the scale up/down animation takes")]
        public float AnimateDuration = 0.4f;
        [Tooltip("How long the prompt shoes at max size for")]
        public float ShowDuration = 3f;
        
        private float _startTime;
        private float _endTime;
        private float _lastShowTime;
        private bool _animating;

        public void Awake()
        {
            transform.localScale = Vector3.zero;
        }
        
        public void RegisterStart()
        {
            _startTime = Time.time;
        }

        public void RegisterStop()
        {
            _endTime = Time.time;
            if (_endTime - _startTime < ThresholdDuration && Time.time - _lastShowTime > Cooldown && !_animating) {
                StartCoroutine(AnimatePrompt());
            }
        }

        private IEnumerator AnimatePrompt()
        {
            _animating = true;
            for (var i = 0f; i < AnimateDuration; i += Time.deltaTime / AnimateDuration) {
                transform.localScale = Vector3.one * Ease.CubicOut(i);
                yield return null;
            }
            transform.localScale = Vector3.one;
            yield return new WaitForSeconds(ShowDuration);
            for (var i = 0f; i < AnimateDuration; i += Time.deltaTime / AnimateDuration) {
                transform.localScale = Vector3.one * (1f - Ease.CubicIn(i));
                yield return null;
            }
            transform.localScale = Vector3.zero;
            _lastShowTime = Time.time;
            _animating = false;
        }
    }
}