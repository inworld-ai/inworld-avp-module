using TMPro;
using UnityEngine;

namespace LC.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextCopy : MonoBehaviour
    {
        public TMP_Text Target;
        private TMP_Text _text;
        
        public void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void Update()
        {
            if (Target == null) return;
            _text.text = Target.text;
        }
    }
}