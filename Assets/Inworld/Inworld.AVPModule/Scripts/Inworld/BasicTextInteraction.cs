using System.Collections.Generic;
using Inworld;
using Lunity;
using UnityEngine;

namespace LC.InworldUtils
{
    /// Simple utility class that makes it easy to send text or simple triggers to InWorld from the inspector
    /// Place this script on any object and (assuming you have a live Inworld connection) you can send messages with the
    /// inspector buttons
    /// todo: Make a proper custom editor to make this easier to use
    public class BasicTextInteraction : MonoBehaviour
    {
        public string DebugText;
        public string DebugTrigger;

        [EditorButton]
        public void SendDebugText()
        {
            SendText(DebugText);
            DebugText = "";
        }

        [EditorButton]
        public void SendDebugTrigger()
        {
            SendTrigger(DebugTrigger);
        }

        public void SendText(string text)
        {
            InworldController.Instance.SendText(InworldController.CurrentCharacter.ID, text);
        }

        public void SendTrigger(string triggerName, Dictionary<string, string> parameters = null)
        {
            InworldController.Instance.SendTrigger(triggerName, InworldController.CurrentCharacter.ID, parameters);
        }
    }
}