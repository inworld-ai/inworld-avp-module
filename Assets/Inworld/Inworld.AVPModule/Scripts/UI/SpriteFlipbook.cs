using UnityEngine;
using UnityEngine.UI;

namespace LC.UI
{
    [RequireComponent(typeof(Image))]
    public class SpriteFlipbook : MonoBehaviour
    {
        public Sprite[] Sprites;
        [Range(0f, 1f)] public float TargetPosition = 0f;
        public float Duration = 0.3f;

        private int _currentIndex;
        private float _position;
        private Image _image;
        
        public void Awake()
        {
            _image = GetComponent<Image>();
            _position = TargetPosition;
            _image.sprite = Sprites[_currentIndex];
            _currentIndex = GetSpriteIndex(_position);
        }

        private int GetSpriteIndex(float progress)
        {
            return Mathf.Clamp(Mathf.FloorToInt(progress * Sprites.Length), 0, Sprites.Length - 1);
        }

        public void Update()
        {
            var positionOffset = TargetPosition - _position;
            if (Mathf.Abs(positionOffset) < float.Epsilon) {
                //do nothing
            } else if (positionOffset > 0f) _position += Time.deltaTime / Duration;
            else if(positionOffset < 0f) _position -= Time.deltaTime / Duration;
            _position = Mathf.Clamp01(_position);
            var newIndex = GetSpriteIndex(_position);
            if (newIndex == _currentIndex) return;
            _currentIndex = newIndex;
            _image.sprite = Sprites[newIndex];
        }
    }
}