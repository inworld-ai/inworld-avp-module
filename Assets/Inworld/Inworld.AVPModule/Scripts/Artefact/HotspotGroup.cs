using System.Collections.Generic;
using Lunity;
using UnityEngine;

namespace LC.Artefact
{
    /// Ensures that only one hotspot can be open at once, closing the currently-open one when a new one is opened.
    public class HotspotGroup : MonoBehaviour
    {
        public List<Hotspot> Hotspots;
        [ReadOnly] public Hotspot CurrentlyOpenHotspot;

        private AntiquinntyScene _scene;
        
        public void Awake()
        {
            _scene = FindObjectOfType<AntiquinntyScene>();
            foreach (var hs in Hotspots) hs.SetGroup(this);
        }

        public void OpenHotspot(Hotspot hotspot)
        {
            if (CurrentlyOpenHotspot != null && CurrentlyOpenHotspot.IsAnimating) return;
            CloseCurrentHotspot();
            CurrentlyOpenHotspot = hotspot;
            CurrentlyOpenHotspot.SetOpen();
            _scene.RespondToHotspot(hotspot);
        }

        public void CloseCurrentHotspot()
        {
            if (CurrentlyOpenHotspot != null) {
                if (CurrentlyOpenHotspot.IsAnimating) return;
                CurrentlyOpenHotspot.SetClosed();
            }
            CurrentlyOpenHotspot = null;
        }
    }
}