using UnityEngine;

//from https://github.com/lachlansleight/Lunity
namespace Lunity
{
    [ExecuteAlways]
    public class CopyTransform : MonoBehaviour
    {
        public Transform Target;

        [Space(10)]
        public bool CopyPosition = true;

        public bool PositionX = true;
        public bool PositionY = true;
        public bool PositionZ = true;

        [Space(10)]
        public bool CopyRotation = true;

        public bool RotationX = true;
        public bool RotationY = true;
        public bool RotationZ = true;

        [Space(10)]
        public bool CopyScale = true;

        public bool ScaleX = true;
        public bool ScaleY = true;
        public bool ScaleZ = true;


        private Transform _t;
        private bool _hasT;

        public void Awake()
        {
            _t = transform;
        }

        public void LateUpdate()
        {
            if (Target == null) return;
            if (_hasT) {
                _t = transform;
                _hasT = true;
            }

            if (CopyPosition) {
                var cPos = _t.position;
                var tPos = Target.position;
                _t.position = new Vector3(
                    PositionX ? tPos.x : cPos.x,
                    PositionY ? tPos.y : cPos.y,
                    PositionZ ? tPos.z : cPos.z
                );
            }

            if (CopyRotation) {
                var cRot = _t.eulerAngles;
                var tRot = Target.eulerAngles;
                _t.eulerAngles = new Vector3(
                    RotationX ? tRot.x : cRot.x,
                    RotationY ? tRot.y : cRot.y,
                    RotationZ ? tRot.z : cRot.z
                );
            }

            if (CopyScale) {
                var cScale = _t.localScale;
                var tScale = Target.localScale;
                _t.localScale = new Vector3(
                    ScaleX ? tScale.x : cScale.x,
                    ScaleY ? tScale.y : cScale.y,
                    ScaleZ ? tScale.z : cScale.z
                );
            }
        }
    }
}