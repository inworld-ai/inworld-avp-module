using System.Collections;
using Lunity;
using UnityEngine;

namespace LC
{
    /// Simple Utility class to remove the hat that we pull off Innequin's head and place onto the 'X' target
    public class ShrinkAndRemoveHat : MonoBehaviour
    {
        private bool _animating;

        public void ShrinkAndRemove()
        {
            if (_animating) return;
            StartCoroutine(ShrinkAndRemoveRoutine());
        }

        private IEnumerator ShrinkAndRemoveRoutine()
        {
            _animating = true;
            var startScale = transform.localScale;
            for (var i = 0f; i < 1f; i += Time.deltaTime / 0.5f) {
                transform.localScale = Vector3.Lerp(startScale, Vector3.one * 0.00001f, Ease.CubicIn(i));
                yield return null;
            }

            //begone - the scene is about to unload anyway :D
            transform.position = Vector3.down * 1000f;
        }
    }
}