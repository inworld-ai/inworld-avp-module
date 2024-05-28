using LC.InworldUtils;
using LC.UI;
using Lunity;
using UnityEngine;

namespace LC.DebugUtils
{
    public class DebugSceneInterface : MonoBehaviour
    {
        public string Text;
        public string Trigger;

        [EditorButton]
        public void SendText()
        {
            if (string.IsNullOrEmpty(Text)) return;
            InworldAsyncUtils.SendText(Text);
            Text = "";
        }

        [EditorButton]
        public void SendTrigger()
        {
            if (string.IsNullOrEmpty(Trigger)) return;
            InworldAsyncUtils.SendTrigger(Trigger);
            Trigger = "";
        }

        [EditorButton]
        public void SetHatNone()
        {
            FindObjectOfType<HatSwitcher>().SetHat(HatSwitcher.Hat.None);
        }

        [EditorButton]
        public void SetHatGameShow()
        {
            FindObjectOfType<HatSwitcher>().SetHat(HatSwitcher.Hat.GameShow);
        }

        [EditorButton]
        public void SetHatArtefact()
        {
            FindObjectOfType<HatSwitcher>().SetHat(HatSwitcher.Hat.Artefact);
        }

        [EditorButton]
        public void SetHatFortune()
        {
            FindObjectOfType<HatSwitcher>().SetHat(HatSwitcher.Hat.Fortune);
        }

        [EditorButton]
        public void ToggleInputMenu()
        {
            FindObjectOfType<MainInputCanvas>().DebugToggle();
        }

        [EditorButton]
        public void ToggleImmersiveMode()
        {
            FindObjectOfType<ImmersiveToggleButton>().ToggleImmersiveMode();
        }
    }
}