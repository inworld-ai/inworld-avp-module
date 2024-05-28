using UnityEngine;

namespace LC.UI
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshFlipbook : MonoBehaviour
    {
        public Mesh[] Meshes;
        [Range(0f, 1f)] public float TargetPosition = 0f;
        [Range(1f, 24f)] public float SmoothSpeed = 16f;

        private Vector2[] _meshBoundaries;
        
        private int _currentIndex;
        private float _position;
        private MeshFilter _mf;
        private Vector3 _defaultScale;
        
        public void Awake()
        {
            _mf = GetComponent<MeshFilter>();
            _position = TargetPosition;
            _mf.mesh = Meshes[_currentIndex];
            _defaultScale = transform.localScale;
            
            _currentIndex = GetSpriteIndex(_position, out var scale);
            scale = Mathf.Lerp(0.925f, 1f, scale);
            transform.localScale = new Vector3(_defaultScale.x * scale, _defaultScale.y, _defaultScale.z);

            _meshBoundaries = new Vector2[Meshes.Length];
            var pos = 0f;
            for (var i = 0; i < _meshBoundaries.Length; i++) {
                var newPos = pos + 1f / _meshBoundaries.Length;
                _meshBoundaries[i] = new Vector2(pos, newPos);
                pos = newPos;
            }
        }

        private int GetSpriteIndex(float progress, out float scale)
        {
            if (progress <= 0f) {
                scale = 1f;
                return 0;
            }

            if (progress >= 1f) {
                scale = 1f;
                return Meshes.Length - 1;
            }
            
            for (var i = 0; i < _meshBoundaries.Length; i++) {
                if (progress >= _meshBoundaries[i].x && progress < _meshBoundaries[i].y) {
                    scale = Mathf.InverseLerp(_meshBoundaries[i].y, _meshBoundaries[i].x, progress);
                    return i;
                }
            }

            Debug.LogWarning("Failed to find valid mesh index");
            scale = 1f;
            return 0;
        }

        public void Update()
        {
            var positionOffset = TargetPosition - _position;
            if (Mathf.Abs(positionOffset) < float.Epsilon) {
                //do nothing
            } else {
                _position = Mathf.Lerp(_position, TargetPosition, Time.deltaTime * SmoothSpeed);
            }
            _position = Mathf.Clamp01(_position);
            var newIndex = GetSpriteIndex(_position, out var scale);
            scale = Mathf.Lerp(0.925f, 1f, scale);
            transform.localScale = new Vector3(_defaultScale.x * scale, _defaultScale.y, _defaultScale.z);
            if (newIndex == _currentIndex) return;
            _currentIndex = newIndex;
            _mf.mesh = Meshes[newIndex];
        }
    }
}