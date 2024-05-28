using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LC.UI
{
    [RequireComponent(typeof(RectTransform), typeof(BoxCollider))]
    public class ButtonHysteresis : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Vector3 ColliderMultiplier = new Vector3(1.2f, 1.2f, 5f);

        private BoxCollider _collider;
        private Vector3 _defaultScale;
        
        public void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _defaultScale = _collider.size;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            var newScale = _defaultScale;
            newScale.Scale(ColliderMultiplier);
            _collider.size = newScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _collider.size = _defaultScale;
        }
    }
}