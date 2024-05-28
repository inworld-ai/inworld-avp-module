using LC.Interaction;
using UnityEngine;

namespace LC.UI
{
    public class LookAtCamera : MonoBehaviour
    {
        public void Update()
        {
            if (!VisionOsImmersiveMode.IsImmersive) return;
            var targetPos = VisionOsTransforms.GetCameraPosition();
            transform.rotation = Quaternion.LookRotation(targetPos - transform.position, Vector3.up);
        }
    }
}