using System.Collections;
using LC.Interaction;
using Lunity;
using UnityEngine;

namespace LC.Tarot
{
    /// Just a snap target that knows what socket name it corresponds to
    public class TarotSpreadSocket : MagneticSnapTarget
    {
        public string SocketName = "Past";
        public float OffsetSinkDuration = 1.5f;

        public override bool SnapObject(Grabbable grabbable, bool suppressEvent = false)
        {
            StartCoroutine(SinkOffsetPosition());
            return base.SnapObject(grabbable, suppressEvent);
        }

        private IEnumerator SinkOffsetPosition()
        {
            var startPos = PositionOffset;
            var targetPos = Vector3.zero;
            for (var i = 0f; i < 1f; i += Time.deltaTime / OffsetSinkDuration) {
                PositionOffset = Vector3.Lerp(startPos, targetPos, Ease.CubicInOut(i));
                yield return null;
            }
            PositionOffset = Vector3.zero;
        }
    }
}