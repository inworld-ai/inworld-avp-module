using UnityEngine;

namespace LC
{
    /// This is a strange script - but we need it to turn off the Innequin character model before it begins to
    /// self-initialize. We could just have it turned off in the scene, but then the scene becomes slightly
    /// harder to use.
    public class DisableOnAwake : MonoBehaviour
    {
        public void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}