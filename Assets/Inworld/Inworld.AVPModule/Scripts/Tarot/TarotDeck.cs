using System.Collections;
using LC.Interaction;
using Lunity;
using UnityEngine;

namespace LC.Tarot
{
    public class TarotDeck : MonoBehaviour
    {
        public GameObject TemplateCard;
        public Transform CardParent;
        /// Where new cards are spawned relative to the deck
        public Vector3 NewCardPosition = new Vector3(0f, 0f, -0.018f);
        /// Euler angles of newly spawned cards
        public Vector3 SpawnRotation = new Vector3(0f, 0f, 0f);
        public GrabbableCard NextCard;
        
        private GameDeckBase<int> _deck;
        private MystiquinnScene _scene;
        private AudioJukebox _audio;
        private Vector3 _defaultScale;
        private bool _firstEnable = true;

        public void Awake()
        {
            _scene = FindObjectOfType<MystiquinnScene>();
            _audio = GetComponent<AudioJukebox>();
            _defaultScale = transform.localScale;
            transform.localScale = Vector3.one * 0.0001f;
            InitializeDeck();
        }

        ///Sets up the deck with all the cards in it, and then shuffles
        public void InitializeDeck()
        {
            _deck = new GameDeckBase<int>();
            for (var suit = 0; suit <= 400; suit += 100) {
                for (var rank = 1; rank <= 22; rank++) {
                    if (suit > 0 && rank > 14) break;
                    var id = suit + rank;
                    _deck.AddItemToTop(id);
                }
            }

            _deck.Shuffle();
        }

        public bool ShowDeckForFirstDraw()
        {
            if (_firstEnable) {
                _firstEnable = false;
                StartCoroutine(FirstEnableRoutine());
                return true;
            }

            return false;
        }

        private IEnumerator FirstEnableRoutine()
        {
            var startScale = transform.localScale;
            for (var i = 0f; i < 1f; i += Time.deltaTime / 1f) {
                transform.localScale = Vector3.Lerp(startScale, _defaultScale, Ease.CubicOut(i));
                yield return null;
            }

            transform.localScale = _defaultScale;
            SpawnFreshCard();
            NextCard.IsGrabbable = true;
        }

        /// Draws a card from the deck and places it on top, face-down, ready to be picked up
        public void SpawnFreshCard(bool playAudio = false)
        {
            if (_deck == null || _deck.Count == 0) InitializeDeck();
            
            var newCard = SpawnTopCard();
            newCard.transform.position = transform.TransformPoint(NewCardPosition);
            newCard.transform.localRotation = Quaternion.Euler(SpawnRotation);
            newCard.GetComponent<GrabbableCard>().Initialize(this, _scene);
            
            if(playAudio) _audio.Play();
        }

        /// Returns a card to the bottom of the deck
        public void AddCardToDeck(int cardId)
        {
            _deck.AddItemToBottom(cardId);
        }

        /// Actually instantiates a card object and sets it up
        public TarotCard SpawnTopCard()
        {
            if (_deck == null || _deck.Count == 0) InitializeDeck();

            var newObj = Instantiate(TemplateCard);
            var card = newObj.GetComponent<TarotCard>();
            card.Card = Card.GetCardFromId(_deck.TakeFromTop());
            card.transform.parent = CardParent;
            card.transform.localScale = transform.localScale;
            NextCard = card.GetComponent<GrabbableCard>();
            return card;
        }

#if UNITY_EDITOR
        [EditorButton]
        public void DrawCardAndGrab()
        {
            var newCard = SpawnTopCard();
            newCard.transform.localRotation = Quaternion.Euler(SpawnRotation);
            var grabbable = newCard.GetComponent<GrabbableCard>();
            grabbable.CurrentSnapTarget = GetComponent<MagneticSnapTarget>();
            grabbable.DrawFromDeck(transform.TransformPoint(NewCardPosition));
        }

        [EditorButton]
        public void DrawCardDebug()
        {
            var card = SpawnTopCard();
            card.transform.position = transform.position + Random.insideUnitSphere * 0.1f;
            card.transform.localRotation = Quaternion.Euler(SpawnRotation);
        }
#endif
    }
}