using System.Collections;
using System.Collections.Generic;
using Inworld;
using LC.Interaction;
using LC.InworldUtils;
using UnityEngine;

namespace LC.Artefact
{
    public class AntiquinntyScene : GdcScene
    {
        
        private IKLookAttention _lookAttention;
        
        //Inneqin looks at the chosen hotspot and speaks about it
        public void RespondToHotspot(Hotspot hotspot)
        {
            StopAllCoroutines();
            InworldController.CurrentCharacter.CancelResponse();
            InworldAsyncUtils.SendTrigger("hotspot", "description", hotspot.Description);
            StartCoroutine(LookAtHotspot(hotspot.transform));
        }

        private IEnumerator LookAtHotspot(Transform hotspot)
        {
            if (_lookAttention == null) {
                _lookAttention = FindObjectOfType<IKLookAttention>();
            }
            _lookAttention.ObjectTarget = hotspot;
            _lookAttention.CurrentTarget = IKLookAttention.AttentionTargetType.Object;
            yield return new WaitForSeconds(5f);
            _lookAttention.CurrentTarget = VisionOsImmersiveMode.IsImmersive 
                ? IKLookAttention.AttentionTargetType.Player
                : IKLookAttention.AttentionTargetType.Idle;
        }
    }
}