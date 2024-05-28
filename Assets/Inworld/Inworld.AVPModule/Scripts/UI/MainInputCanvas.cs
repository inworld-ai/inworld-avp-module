using System;
using System.Collections;
using LC.InworldUtils;
using Lunity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LC.UI
{
    /// Temporary class to manage the behaviour of the placeholder UI
    public class MainInputCanvas : MonoBehaviour
    {
        public float AnimateDuration = 0.3f;
        public float ButtonOpenOffset = 50f;

        private RectTransform _buttonParent;
        private Button _toggleInputButton;
        private TMP_InputField _inputField;
        private Button _sendTextButton;

        private CanvasGroup _mainCanvasGroup;
        private CanvasGroup _textInputContainer;

        public bool ShowingText;
        private bool _animating;

        public Action<float> OnTextFieldAlphaChange;
        public Action<bool> OnEnableTextField;

        public void Awake()
        {
            _toggleInputButton = transform.Find("LeftPanel/TranscriptButton").GetComponent<Button>();

            //disable the main canvas group while we wait for the main connection routine to occur
            _mainCanvasGroup = GetComponent<CanvasGroup>();
            _mainCanvasGroup.alpha = 0f;

            _buttonParent = transform.Find("LeftPanel").GetComponent<RectTransform>();
            _textInputContainer = transform.Find("TextField").GetComponent<CanvasGroup>();
            _inputField = _textInputContainer.transform.Find("InputField").GetComponent<TMP_InputField>();
            _sendTextButton = _textInputContainer.transform.Find("SendButton").GetComponent<Button>();

            _buttonParent.anchoredPosition = new Vector2(ButtonOpenOffset, 0f);

            _toggleInputButton.onClick.AddListener(() => StartCoroutine(AnimateText(!ShowingText)));
            _sendTextButton.onClick.AddListener(() =>
            {
                InworldAsyncUtils.SendText(_inputField.text);
                _inputField.text = "";
            });
            _inputField.onValueChanged.AddListener(text => _sendTextButton.interactable = !string.IsNullOrEmpty(text));
            
            _textInputContainer.alpha = 0f;
            _inputField.gameObject.SetActive(false);
            _sendTextButton.gameObject.SetActive(false);
        }

        [ContextMenu("Debug Toggle")]
        public void DebugToggle()
        {
            StartCoroutine(AnimateText(!ShowingText));
        }

        private IEnumerator AnimateText(bool toOpen)
        {
            void SetWithLerp(float t)
            {
                if (toOpen) {
                    _buttonParent.anchoredPosition = new Vector2(Mathf.Lerp(ButtonOpenOffset, 0f, Ease.CubicInOut(t)), 0f);
                    //_textInputContainer.alpha = t;
                    //_buttonParent.anchoredPosition = new Vector2(Mathf.Lerp(ButtonOpenOffset, 0f, Ease.CubicInOut(Mathf.Clamp01(t / 0.5f))), 0f);
                    _textInputContainer.alpha = Mathf.Clamp01((t - 0.5f) / 0.5f);
                } else {
                    _buttonParent.anchoredPosition = new Vector2(Mathf.Lerp(ButtonOpenOffset, 0f, 1f - Ease.CubicInOut(t)), 0f);
                    //_textInputContainer.alpha = 1f - t;
                    //_buttonParent.anchoredPosition = new Vector2(Mathf.Lerp(ButtonOpenOffset, 0f, 1f - Ease.CubicInOut(Mathf.Clamp01(t - 0.5f) / 0.5f)), 0f);
                    _textInputContainer.alpha = 1f - Mathf.Clamp01(t / 0.5f);
                }
                OnTextFieldAlphaChange?.Invoke(_textInputContainer.alpha);
            }

            if (_animating) yield break;

            if (toOpen) {
                _sendTextButton.interactable = true;
                _inputField.interactable = true;
                _inputField.gameObject.SetActive(true);
                _sendTextButton.gameObject.SetActive(true);
                OnEnableTextField?.Invoke(true);
            }

            _animating = true;
            for (var i = 0f; i < 1f; i += Time.deltaTime / AnimateDuration) {
                var t = toOpen ? Ease.CubicOut(i) : Ease.CubicIn(i);
                SetWithLerp(i);
                yield return null;
            }

            SetWithLerp(1f);
            _animating = false;
            ShowingText = toOpen;
            if (!toOpen) {
                _sendTextButton.interactable = false;
                _inputField.interactable = false;
                _inputField.text = "";
                _inputField.gameObject.SetActive(false);
                _sendTextButton.gameObject.SetActive(false);
                OnEnableTextField?.Invoke(false);
            }
        }
    }
}