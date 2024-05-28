using LC.Interaction;
using LC.InworldUtils;
using TMPro;
using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LC.UI
{
    /// Unity canvas button that toggles bounded/unbounded mode
    public class ImmersiveToggleButton : MonoBehaviour, IPointerUpHandler
    {
        public Sprite SpriteWhenBounded;
        public Sprite SpriteWhenUnbounded;

        public Color TextColorWhenBounded = Color.white;
        public Color TextColorWhenUnbounded = Color.black;
        
        private VolumeCamera _volume;

        private TMP_Text _text;
        private Image _image;
        private bool _bounded = true;

        public void Awake()
        {
            _text = GetComponentInChildren<TMP_Text>();
            _image = GetComponent<Image>();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ToggleImmersiveMode();
        }

        public void ToggleImmersiveMode()
        {
            VisionOsImmersiveMode.Instance.ToggleMode();
            _bounded = !_bounded;
            _image.sprite = _bounded ? SpriteWhenBounded : SpriteWhenUnbounded;
            _text.color = _bounded ? TextColorWhenBounded : TextColorWhenUnbounded;

            var loadedScenes = SceneManager.sceneCount;
            var inHomeScene = false;
            for (var i = 0; i < loadedScenes; i++) {
                if (SceneManager.GetSceneAt(i).name == "1_Home") inHomeScene = true;
            }
            
            //Tells Innequin that we switched mode
            if(inHomeScene) InworldAsyncUtils.SendTrigger(_bounded ? "volume" : "immersive");
        }
    }
}