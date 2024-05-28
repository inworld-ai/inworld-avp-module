using UnityEngine;

namespace LC.UI
{

    [ExecuteAlways, RequireComponent(typeof(Renderer))]
    public class WorldSpaceLine : MonoBehaviour
    {
        public Space PositionSpace = Space.World;
        public Vector3 StartPosition;
        public Vector3 TargetPosition;
        [Range(0f, 1f)] public float Progress;
        public float Width = 0.01f;
        public bool Render = true;

        private Renderer _renderer;
        
        public void LateUpdate()
        {
            if (_renderer == null) {
                _renderer = GetComponent<Renderer>();
            }
            _renderer.enabled = Render && Progress > 0f;

            transform.position = TransformPosition(StartPosition);
            var offset = (TransformPosition(TargetPosition) - transform.position + Vector3.up * 0.0001f);
            var length = offset.magnitude * Progress;
            transform.rotation = Quaternion.LookRotation(offset);
            transform.localScale = new Vector3(Width, Width, length);
        }

        private Vector3 TransformPosition(Vector3 position)
        {
            if (PositionSpace == Space.World) return position;
            if (transform.parent == null) return position;
            return transform.parent.TransformPoint(position);
        }
    }
}