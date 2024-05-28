using Inworld;
using Inworld.Packet;
using Lunity;
using UnityEngine;

namespace LC.InworldUtils
{
    public class Icebreaker : MonoBehaviour
    {
        public float FirstIcebreakerSilenceTimeRequirement = 30f;
        public float SilenceTimeRequirement = 10f;
        public bool CanSendMultipleConsecutiveIcebreakers = false;
        [ReadOnly] public bool SentIcebreakerThisTurn = false;
        private float _lastInteractionTime;
        private bool _hasFirstInteraction;
        private bool _sentFirstIcebreaker;

        public void Awake()
        {
            InworldCharacterEvents.OnText += HandleText;
        }

        private void HandleText(TextPacket packet)
        {
            _hasFirstInteraction = true;
            _lastInteractionTime = Time.time;
            
            if(packet.routing.source.type.ToUpper() == "PLAYER") SentIcebreakerThisTurn = false;
        }

        public void Update()
        {
            if (InworldController.Audio.IsPlayerSpeaking) _lastInteractionTime = Time.time;

            if (!_hasFirstInteraction) return;
            if (SentIcebreakerThisTurn && !CanSendMultipleConsecutiveIcebreakers) return;
            if (_sentFirstIcebreaker) {
                if (Time.time < _lastInteractionTime + SilenceTimeRequirement) return;
            } else {
                if (Time.time < _lastInteractionTime + FirstIcebreakerSilenceTimeRequirement) return;
            }

            Debug.Log("Sending icebreaker!");
            InworldAsyncUtils.SendTrigger("icebreaker");
            _lastInteractionTime = Time.time;
            SentIcebreakerThisTurn = true;
            _sentFirstIcebreaker = true;
        }
    }
}