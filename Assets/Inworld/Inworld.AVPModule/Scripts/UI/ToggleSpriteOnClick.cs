using UnityEngine;
using UnityEngine.UI;

namespace LC.UI
{
    [RequireComponent(typeof(Button), typeof(Image))]
    public class ToggleSpriteOnClick : MonoBehaviour
    {
        public Sprite AlternateSprite;
        private bool _isAlternate;

        public void Awake()
        {
            var image = GetComponent<Image>();
            var defaultSprite = image.sprite;
            GetComponent<Button>().onClick.AddListener(() =>
            {
                _isAlternate = !_isAlternate;
                image.sprite = _isAlternate ? AlternateSprite : defaultSprite;
            });
        }
    }
}