using LC.Interaction;
using UnityEngine;

namespace LC.Artefact
{
    //Increases the scale of the turntable handle while it's being pressed, to avoid it flickering on and off when it moves
    [RequireComponent(typeof(BoxCollider), typeof(TouchReceiver))]
    public class HandleHysteresis : MonoBehaviour
    {
        private TouchReceiver _touchReceiver;
        private BoxCollider _boxCollider;
        
        public Vector3 ColliderScale = new Vector3(1f, 1f, 10f);

        private Vector3 _defaultScale;
        private Vector3 _touchedScale;
        
        public void Start()
        {
            _boxCollider = GetComponent<BoxCollider>();
            _touchReceiver = GetComponent<TouchReceiver>();
            _defaultScale = _boxCollider.size;
            _touchedScale = _defaultScale;
            _touchedScale.Scale(ColliderScale);
            _touchReceiver.OnTouchDown.AddListener(_ => _boxCollider.size = _touchedScale);
            _touchReceiver.OnTouchUp.AddListener(() => _boxCollider.size = _defaultScale);
        }
    }
}