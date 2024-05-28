using System.Collections;
using System.Collections.Generic;
using Inworld;
using LC.InworldUtils;
using UnityEngine;

namespace LC
{
    public class HomeScene : GdcScene
    {
        public string FirstTimeTrigger = "first_intro";
        public string GeneralTrigger = "welcome";

        private static bool EverWelcomed;

        protected override void SendWelcomeMessage()
        {
            var message = EverWelcomed ? GeneralTrigger : FirstTimeTrigger;
            InworldAsyncUtils.SendTrigger(message);
            EverWelcomed = true;
        }
    }
}