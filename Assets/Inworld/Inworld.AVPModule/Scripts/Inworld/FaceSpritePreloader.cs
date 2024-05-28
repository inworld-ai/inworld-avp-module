using System.Collections;
using Inworld.Sample.Innequin;
using UnityEngine;

namespace LC.InworldUtils
{
    public class FaceSpritePreloader : MonoBehaviour
    {
        public FaceTransformData FaceTransformData;
        private Material _material;

        public IEnumerator RunthroughSprites()
        {
            _material = GetComponent<Renderer>().material;
            foreach (var data in FaceTransformData.data) {
                _material.mainTexture = data.eyeBlow;
                yield return null;
                _material.mainTexture = data.eye;
                yield return null;
                _material.mainTexture = data.eye;
                yield return null;
                _material.mainTexture = data.eye;
                yield return null;
                _material.mainTexture = data.eye;
                yield return null;
                foreach (var tex in data.mouth) {
                    _material.mainTexture = tex;
                    yield return null;
                }
            }
        }
    }
}