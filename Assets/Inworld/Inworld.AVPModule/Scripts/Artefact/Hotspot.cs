using System.Collections;
using LC.Interaction;
using Lunity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LC.Artefact
{
    /// Class controlling the behaviour and appearance of the hotspot targets on the artefact.
    /// Controls the appearance of the dot, as well as the canvas visibility, positioning and animation.
    public class Hotspot : MonoBehaviour
    {

        public string Description;
        public Transform HotspotCenter;
        public float AnimateDuration = 1f;
        [Range(0f, 1f)] public float AnimateShoulder = 0.4f;
        public float CanvasRadius = 0.1f;
        public float BottomSpacing = 0.02f;
        public bool IsOpen;

        [HideInInspector] public HotspotGroup Group;

        public bool IsAnimating;
        private Image _titleLine;
        private RectTransform _canvas;
        private CanvasGroup _canvasGroup;
        private TMP_Text _paragraph;
        private TouchReceiver _touchReceiver;

        private Transform _dotTargetT;
        private Material _dotMat;
        private Color _dotCol;
        private Vector3 _initDotScale;

        private bool _initialized;
        
        public void Awake()
        {
            Initialize();
        }

        public void SetGroup(HotspotGroup group)
        {
            Initialize();
            Group = group;
            _touchReceiver.OnTouchDown.AddListener(_ =>
            {
                if (Group.CurrentlyOpenHotspot == this) Group.CloseCurrentHotspot();
                else Group.OpenHotspot(this);
            });
        }

        private void Initialize()
        {
            if (_initialized) return;
            
            _titleLine = transform.Find("Canvas/TitleLine").GetComponent<Image>();
            _canvas = transform.Find("Canvas").GetComponent<RectTransform>();
            _paragraph = _canvas.Find("Paragraph").GetComponent<TMP_Text>();
            _canvasGroup = _canvas.GetComponent<CanvasGroup>();
            _touchReceiver = transform.Find("Target").GetComponent<TouchReceiver>();
            _dotTargetT = transform.Find("Target").GetComponent<Transform>();
            _dotMat = Application.isPlaying 
                ? _dotTargetT.GetComponent<Renderer>().material 
                : _dotTargetT.GetComponent<Renderer>().sharedMaterial;
            _dotCol = _dotMat.color;
            _initDotScale = _dotTargetT.localScale;
            _initialized = true;

            SetClosedNow();
        }

        [EditorButton]
        public void SetOpenNow()
        {
            SetNow(true);
        }

        [EditorButton]
        public void SetClosedNow()
        {
            SetNow(false);
        }
        
        public void SetNow(bool open)
        {
            Initialize();
            
            StopAllCoroutines();
            IsAnimating = false;
            
            IsOpen = open;
            SetOpenProgress(open ? 1f : 0f);
        }

        private void SetOpenProgress(float progress)
        {
            _titleLine.fillAmount = Mathf.Clamp01((progress - AnimateShoulder) / (1f - AnimateShoulder));
            _canvasGroup.alpha = Mathf.Clamp01((progress - AnimateShoulder) / (1f - AnimateShoulder));

            _dotMat.color = Color.Lerp(_dotCol, Color.white, progress);
            _dotTargetT.localScale = Vector3.Lerp(_initDotScale, _initDotScale / 2, Ease.CubicOut(progress));
        }

        private void SetClosedProgress(float progress)
        {
            _titleLine.fillAmount = 1f - Mathf.Clamp01(progress / (1f - AnimateShoulder));
            _canvasGroup.alpha = 1f - Mathf.Clamp01(progress / (1f - AnimateShoulder));

            _dotMat.color = Color.Lerp(_dotCol, Color.white, 1f - progress);
            _dotTargetT.localScale = Vector3.Lerp(_initDotScale / 2, _initDotScale, Ease.CubicOut(progress));
        }

        [EditorButton]
        public void SetOpen()
        {
            Set(true);
        }

        [EditorButton]
        public void SetClosed()
        {
            Set(false);
        }
        
        public void Set(bool open)
        {
            if (IsAnimating) return;
            StartCoroutine(SetRoutine(open));
        }

        private void SetIsLeft(bool isLeft)
        {
            _titleLine.fillOrigin = isLeft ? 0 : 1;
        }

        private IEnumerator SetRoutine(bool open)
        {
            IsAnimating = true;
            for (var i = 0f; i < 1f; i += Time.deltaTime / AnimateDuration) {
                var t = open ? Ease.CubicOut(i) : Ease.CubicIn(i);
                if (open) SetOpenProgress(i);
                else SetClosedProgress(i);
                yield return null;
            }
            if (open) SetOpenProgress(1f);
            else SetClosedProgress(1f);
            IsAnimating = false;
        }

        public void Update()
        {
            Initialize();

            var isLeft = (transform.position - HotspotCenter.position).x > 0f;
            SetIsLeft(isLeft);
            
            var canvasSize = new Vector2(_canvas.sizeDelta.x, _paragraph.rectTransform.sizeDelta.y);
            _canvas.sizeDelta = canvasSize;
            canvasSize.Scale(_canvas.lossyScale);
            
            var xPos = HotspotCenter.position.x + (canvasSize.x * 0.5f + CanvasRadius) * (isLeft ? 1f : -1f);
            var yPos = Mathf.Max(HotspotCenter.position.y + BottomSpacing + canvasSize.y * 0.5f, transform.position.y - canvasSize.y * 0.5f);
            var canvasPos = transform.position;
            canvasPos.x = xPos;
            canvasPos.y = yPos;
            _canvas.position = canvasPos;
            _canvas.rotation = HotspotCenter.rotation;

        }
    }
}