using System.Collections;
using Lunity;
using UnityEngine;

namespace LC.Tarot
{
    public class CardBorder : MonoBehaviour
    {
        public Color RestingColor = Color.yellow;
        public Color RestingGlowColor = Color.yellow;
        public Color HighlightColor = Color.white;
        
        [Range(0f, 1f)] public float FlashGlowStrength = 1f;
        [Range(1f, 24f)] public float SmoothSpeed = 8f;
        
        [Space(10f)]
        [Range(0f, 1f)] public float TargetGlowStrength = 0f;
        [Range(0f, 1f)] public float CurrentGlowStrength = 0f;
        [Range(0f, 1f)] public float TargetColorStrength = 0f;
        [Range(0f, 1f)] public float CurrentColorStrength = 0f;

        private Material _border;
        private Material _glow;
        private Material _text;
        private Transform _textTransform;
        private Vector3 _defaultTextScale;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public void Awake()
        {
            _border = transform.Find("Frame").GetComponent<Renderer>().material;
            _glow = transform.Find("Glow").GetComponent<Renderer>().material;
            _text = transform.Find("Text")?.GetComponent<Renderer>()?.material;

            if (_text) {
                _textTransform = transform.Find("Text");
                _defaultTextScale = _textTransform.localScale;
                _textTransform.localScale = Vector3.one * 0.0001f;
            }

            _border.SetColor(EmissionColor, Color.black);
            _glow.SetColor(EmissionColor, Color.black);
        }

        public void ShowText()
        {
            StartCoroutine(ShowTextRoutine());
        }

        private IEnumerator ShowTextRoutine()
        {
            var startScale = _textTransform.localScale;
            for (var i = 0f; i < 1f; i += Time.deltaTime / 1f) {
                _textTransform.localScale = Vector3.Lerp(startScale, _defaultTextScale, Ease.CubicOut(i));
                yield return null;
            }
            _textTransform.localScale = _defaultTextScale;
        }

        public void Update()
        {
            CurrentGlowStrength = Mathf.Lerp(CurrentGlowStrength, TargetGlowStrength, Time.deltaTime * SmoothSpeed);
            CurrentColorStrength = Mathf.Lerp(CurrentColorStrength, TargetColorStrength, Time.deltaTime * SmoothSpeed);

            var color = Color.Lerp(RestingColor, HighlightColor, CurrentColorStrength);
            var glowColor = Color.Lerp(RestingGlowColor, HighlightColor, CurrentColorStrength);
            _border.color = color;
            _glow.color = Color.black;
            if (_text) _text.color = color;
            
            _border.SetColor(EmissionColor, Color.Lerp(Color.black, color, CurrentGlowStrength));
            _glow.SetColor(EmissionColor, Color.Lerp(Color.black, Color.Lerp(Color.black, glowColor, Mathf.Lerp(FlashGlowStrength, 1f, CurrentColorStrength)), CurrentGlowStrength));
        }
    }
}