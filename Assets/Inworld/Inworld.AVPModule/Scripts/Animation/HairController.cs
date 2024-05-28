using LC.Interaction;
using UnityEngine;

namespace LC
{
    /// Utility class to control the voluminousness of Innequin's hair based on what hat she's wearing
    [ExecuteAlways]
    public class HairController : MonoBehaviour
    {
        [System.Serializable]
        public class HatHair
        {
            public GameObject Hair;
            public Grabbable Hat;

            public bool ControlActivity(Vector3 headPosition, float thresholdDistance) {
                var offset = (headPosition - Hat.ThisTransform.position).magnitude;
                if (offset < thresholdDistance) {
                    Hair.SetActive(true);
                    return true;
                }

                Hair.SetActive(false);
                return false;
            }
        }

        public HatHair[] Hats;
        public Transform HeadPosition;
        public GameObject DefaultHair;
        public float ThresholdDistance = 0.04f;

        public void Update()
        {
            var isHatNear = false;
            foreach (var hat in Hats) {
                isHatNear = isHatNear || hat.ControlActivity(HeadPosition.position, ThresholdDistance);
            }
            DefaultHair.SetActive(!isHatNear);
        }
    }
}