using System.Collections;
using LC.Interaction;
using Lunity;
using TMPro;
using UnityEngine;

namespace LC.DebugUtils
{
    public class HandGestureDebugTexts : MonoBehaviour
    {
        private GameObject _template;

        private string[] _gestureNames = {
            "Thumbs Up",
            "Thumbs Down",
            "OK"
        };

        public void Awake()
        {
            _template = transform.GetChild(0).gameObject;
            _template.SetActive(false);
        }

        public void ReceiveGestureLeft(string gestureName)
        {
            StartCoroutine(ShowGestureIndicator(gestureName, VisionOsTransforms.GetLeftHandPosition() + Vector3.up * 0.15f));
        }

        public void ReceiveGestureRight(string gestureName)
        {
            StartCoroutine(ShowGestureIndicator(gestureName, VisionOsTransforms.GetRightHandPosition() + Vector3.up * 0.15f));
        }

        private IEnumerator ShowGestureIndicator(string gestureName, Vector3 position)
        {
            var newObj = Instantiate(_template);
            var targetScale = newObj.transform.localScale;
            newObj.transform.localScale = Vector3.one * 0.000001f;
            var curScale = newObj.transform.localScale;
            newObj.SetActive(true);
            newObj.GetComponentInChildren<TMP_Text>().text = gestureName;
            newObj.transform.position = position;
            newObj.transform.LookAt(VisionOsTransforms.GetCameraPosition());
            newObj.transform.Rotate(0f, 180f, 0f, Space.Self);
            for (var i = 0f; i < 1f; i += Time.deltaTime / 0.3f) {
                newObj.transform.localScale = Vector3.Lerp(curScale, targetScale, Ease.CubicOut(i));
                yield return null;
            }

            var canvasGroup = newObj.GetComponent<CanvasGroup>();
            for (var i = 0f; i < 1f; i += Time.deltaTime / 3f) {
                canvasGroup.alpha = 1f - i;
                newObj.transform.localScale = Vector3.Lerp(targetScale, targetScale * 0.7f, Ease.CubicIn(i));
                yield return null;
            }

            Destroy(newObj);
        }
    }
}