using System.Collections;
using System.Collections.Generic;
using Inworld;
using Inworld.Packet;
using LC.Interaction;
using LC.InworldUtils;
using Lunity;
using UnityEngine;

namespace LC.Tarot
{
    public class MystiquinnScene : GdcScene
    {
        
        public List<(string, string)> PlacedCards;
        public TarotDeck Deck;
        public CardBorder DeckBorder;
        public CardBorder[] Borders;
        public TarotSpreadSocket[] Sockets;
        public Transform TableSurface;

        private bool _inWelcome = true;
        private bool _inReading = false;
        private int _enableDeckCountdown = 0;

        public void Awake()
        {
            PlacedCards = new List<(string card, string position)>();
        }

        protected override void LoadingComplete()
        {
            InworldCharacterEvents.OnControl += HandleControlPacket;
            SetDeckActive(false);
        }

        public void TryHighlightActiveBorder(Vector3 cardPosition)
        {
            var currentTarget = TouchInteractionSystem.GetClosestTarget(cardPosition);
            if (currentTarget == null) {
                for (var i = 0; i < Sockets.Length; i++) {
                    if (Sockets[i].SnappedObjects.Count == 0) Borders[i].TargetColorStrength = 0f;
                }
            }
            for (var i = 0; i < Sockets.Length; i++) {
                if (Sockets[i].SnappedObjects.Count > 0) Borders[i].TargetColorStrength = 1f;
                else if (Sockets[i] == currentTarget) Borders[i].TargetColorStrength = 1f;
                else Borders[i].TargetColorStrength = 0f;
            }
        }

        //This method fires each time Mystiquinn finishes an action within one of her goals
        //We set the deck countdown to two when we place a card. One control packet will come in after we change the scene
        //The second will be fired after Mystiquinn's first voice line within the reading for that card placement,
        //at which time we re-enable the deck
        private void HandleControlPacket(ControlPacket controlPacket)
        {
            //special case:
            //if we're within the first, welcome trigger, we enable the deck at the end of Mystiquinn's first line of dialogue
            if (_inWelcome) {
                SetDeckActive(true);
                _inWelcome = false;
            }

            if (!_inReading) return;
            
            if (_enableDeckCountdown == 0) return;
            _enableDeckCountdown--;
            if (_enableDeckCountdown == 0) {
                SetDeckActive(true);
                _inReading = false;
            }
        }


        public void RegisterCardGrabbed()
        {
            for (var i = 0; i < Borders.Length; i++) {
                if (Sockets[i].SnappedObjects.Count == 0) Borders[i].TargetGlowStrength = 1f;
            }

            SetDeckActive(false);
        }

        public void RegisterCardReleased()
        {
            for (var i = 0; i < Borders.Length; i++) {
                if (Sockets[i].SnappedObjects.Count == 0) Borders[i].TargetGlowStrength = 0f;
            }

            SetDeckActive(true);
        }

        [EditorButton]
        public void DebugAllowCardDraw()
        {
            SetDeckActive(true);
        }

        public void SetDeckActive(bool active)
        {
            Debug.Log("Setting deck active: " + active);
            if (active) {
                if (Deck.ShowDeckForFirstDraw()) {
                    foreach (var border in Borders) {
                        border.ShowText();
                    }
                }
            }
            
            DeckBorder.TargetGlowStrength = active ? 1f : 0f;
            if(Deck.NextCard) Deck.NextCard.IsGrabbable = active;
        }

        public void RespondToCardDraw(string cardName)
        {
            //ignore card draws if we're already speaking - to avoid players drawing many cards resulting in a never-
            //-ending stream of speech from Mystiquinn
            if (InworldController.CurrentCharacter.IsSpeaking) return;
            
            InworldAsyncUtils.SendTrigger("holding_card", "card", cardName);
        }
        
        public void RespondToCardPlaced(string cardName, string socketName)
        {
            PlacedCards.Add((cardName, socketName));
            SetDeckActive(false);
            //we should receive two 'interaction end' control events
            //the first from the 'update_scene' trigger
            //the second from the 'placed_card' trigger (after Mystiquinn has finished giving the initial reading)
            _enableDeckCountdown = 2;

            foreach (var border in Borders) border.TargetGlowStrength = 0f;
            for (var i = 0; i < Borders.Length; i++) {
                if (Sockets[i].SocketName == socketName) Borders[i].TargetColorStrength = 1f;
            }

            Debug.Log(cardName + " placed in slot " + socketName);
            var state = "";
            for (var i = 0; i < PlacedCards.Count; i++) {
                if (i > 0) {
                    if (PlacedCards.Count == 3) state += i == 1 ? ", " : " and ";
                    else state += " and ";
                }

                state += PlacedCards[i].Item1 + " is placed in the " + PlacedCards[i].Item2 + " position";
            }

            state += ".";
            Debug.Log("Set state to " + state);
            InworldAsyncUtils.SendTrigger("update_scene", "state", state);
            InworldController.CurrentCharacter.CancelResponse();
            InworldAsyncUtils.SendTrigger("placed_card", "card", cardName, "position", socketName);

            _inReading = true;

            StartCoroutine(ForceReEnableDeck());
        }

        //Just in case for some reason we don't receive any interaction_end events,
        //this method will re-enable the deck so we can keep progressing through the experience
        private IEnumerator ForceReEnableDeck()
        {
            var timeoutTime = Time.time + 30f;
            while (Time.time < timeoutTime) {
                if (!_inReading) yield break;
                yield return null;
            }
            
            SetDeckActive(true);
            _inReading = false;
        }
    }
}