using System.Collections;
using Lunity;
using UnityEngine;

namespace LC.GameShow
{
    public class GameShowButton : MonoBehaviour
    {
        public CategorySpinner Spinner;
        public float ButtonPushHeight = -0.0237f;

        private Transform _pusher;
        private Vector3 _defaultScale;
        private Vector3 _hiddenScale;
        private Vector3 _defaultPosition;

        private bool _isShowing;

        public void Awake()
        {
            _pusher = transform.Find("Button_Top");
            _defaultScale = transform.localScale;
            _hiddenScale = Vector3.one * 0.0001f;
            _defaultPosition = transform.position;
            
            StartCoroutine(PushRoutine(true));
        }

        public void Push()
        {
            if (!_isShowing) return;
            StartCoroutine(PushRoutine());
        }

        private IEnumerator PushRoutine(bool skipToHidden = false)
        {
            _isShowing = false;

            if (!skipToHidden) {
                yield return StartCoroutine(HideRoutine());
            } else {
                transform.localScale = _hiddenScale;
                transform.localPosition = Vector3.down * 1000f;
                
                //wait a moment for first load to fully complete
                yield return new WaitForSeconds(0.5f);
            }

            while (!Spinner.CanSpin) yield return null;

            yield return StartCoroutine(ShowRoutine());

            _isShowing = true;
        }

        private IEnumerator HideRoutine()
        {
            for (var i = 0f; i < 1f; i += Time.deltaTime / 0.3f) {
                _pusher.localPosition = Vector3.up * ButtonPushHeight * i;
                yield return null;
            }

            Spinner.TriggerSpin();

            for (var i = 0f; i < 1f; i += Time.deltaTime / 0.3f) {
                transform.localScale = Vector3.Lerp(_defaultScale, _hiddenScale, Ease.CubicIn(i));
                yield return null;
            }
            
            transform.localScale = _hiddenScale;
            transform.localPosition = Vector3.down * 1000f;
        }

        private IEnumerator ShowRoutine()
        {
            transform.localPosition = _defaultPosition;
            _pusher.localPosition = Vector3.zero;
            for (var i = 0f; i < 1f; i += Time.deltaTime / 0.3f) {
                transform.localScale = Vector3.Lerp(_hiddenScale, _defaultScale, Ease.CubicOut(i));
                yield return null;
            }
            transform.localScale = _defaultScale;
        }
    }
}