using UnityEngine;
using UnityEngine.UI;

namespace LC.UI
{
    [RequireComponent(typeof(Image))]
    public class SpriteCopy : MonoBehaviour
    {
        public Image Target;
        private Image _image;

        public void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void Update()
        {
            if (Target == null) return;
            _image.sprite = Target.sprite;
        }
    }
}