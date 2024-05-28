using UnityEngine;

namespace LC.UI
{
    /// Copies the alpha of the main input field
    /// Probably temporary
    public class SetTranscriptAlpha : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private MainInputCanvas _mainInputCanvas;

        private void OnEnable()
        {
            _mainInputCanvas = FindObjectOfType<MainInputCanvas>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _canvasGroup.alpha = _mainInputCanvas.ShowingText ? 1f : 0f;
            _mainInputCanvas.OnEnableTextField += HandleEnableChange;
            _mainInputCanvas.OnTextFieldAlphaChange += HandleAlphaChange;
        }

        private void OnDisable()
        {
            _mainInputCanvas.OnEnableTextField -= HandleEnableChange;
            _mainInputCanvas.OnTextFieldAlphaChange -= HandleAlphaChange;
        }

        private void HandleAlphaChange(float alpha)
        {
            _canvasGroup.alpha = alpha;
        }

        private void HandleEnableChange(bool isEnabled)
        {
            _canvasGroup.interactable = isEnabled;
            _canvasGroup.blocksRaycasts = isEnabled;
        }
    }
}