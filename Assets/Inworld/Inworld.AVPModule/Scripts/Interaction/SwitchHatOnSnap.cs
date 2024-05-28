using UnityEngine;

namespace LC.Interaction
{
    /// Snap target that triggers the scene switch
    /// If this is the 'Remove Target', it is the X that appears above Innequin's head
    /// Otherwise this is the snap target on Innequin's head in the main scene
    /// This script also changes the visual appearance of the 'X' based on the hat's distance (if the removable hat is assigned)
    [RequireComponent(typeof(MagneticSnapTarget))]
    public class SwitchHatOnSnap : MonoBehaviour
    {
        public bool IsRemoveTarget;
        public RemovableHat Hat;
        public GameObject NotNearObject;
        public GameObject NearObject;
        
        private HatSwitcher _hatSwitcher;
        private MagneticSnapTarget _snapTarget;

        public void Awake()
        {
            _hatSwitcher = FindObjectOfType<HatSwitcher>();
            _snapTarget = GetComponent<MagneticSnapTarget>();
            _snapTarget.OnObjectSnap.AddListener(OnHatSnap);
        }

        public void Update()
        {
            if (!Hat) return;
            
            if (!Hat.IsGrabbed) {
                NearObject.SetActive(false);
                NotNearObject.SetActive(true);
                return;
            }

            var distance = (_snapTarget.MagnetPosition - Hat.transform.position).magnitude;
            var isNear = distance < _snapTarget.MagneticDistance;
            NotNearObject.SetActive(!isNear);
            NearObject.SetActive(isNear);
        }

        public void OnHatSnap(Grabbable grabbable)
        {
            if (IsRemoveTarget) {
                _hatSwitcher.SetHat(HatSwitcher.Hat.None);
                return;
            }

            //Select the new scene to load based on the name of the GameObject
            switch (grabbable.gameObject.name) {
                case "GameShow":
                    _hatSwitcher.SetHat(HatSwitcher.Hat.GameShow);
                    break;
                case "Fortune":
                    _hatSwitcher.SetHat(HatSwitcher.Hat.Fortune);
                    break;
                case "Artefact":
                    _hatSwitcher.SetHat(HatSwitcher.Hat.Artefact);
                    break;
                default:
                    Debug.LogWarning("Unknown hat dropped onto Innequin: " + grabbable.gameObject.name);
                    break;
            }
        }
    }
}