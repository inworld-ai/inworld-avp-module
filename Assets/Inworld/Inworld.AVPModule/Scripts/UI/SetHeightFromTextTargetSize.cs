using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LC.UI
{
    [ExecuteAlways, RequireComponent(typeof(RectTransform))]
    public class SetHeightFromTextTargetSize : MonoBehaviour
    {
        public TMP_Text Target;
        public float Offset = 0f;
        private RectTransform _rt;
        private RectTransform _parent;

        public void Update()
        {
            if (Target == null) return;
            if (_rt == null) {
                _rt = (RectTransform) transform;
                _parent = (RectTransform) _rt.parent;
            }

            var targetHeight = Target.preferredHeight;
            var size = _rt.sizeDelta;
            size.y = targetHeight + Offset;
            _rt.sizeDelta = size;
            
            LayoutRebuilder.MarkLayoutForRebuild(_parent);
        }
    }
}