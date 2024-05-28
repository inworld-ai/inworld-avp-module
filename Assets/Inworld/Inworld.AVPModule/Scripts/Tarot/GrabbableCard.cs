using System.Collections;
using LC.Interaction;
using Lunity;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

namespace LC.Tarot
{
    public class GrabbableCard : Grabbable
    {
        [Header("Card Config")]
        public float SnapBackDistance = 0.05f;
        public Vector3 OffsetOnGrab = new Vector3(0f, 0.05f, 0f);
        public Vector3 UprightRotation = new Vector3(-60f, 0f, 0f);
        public float VelocityRotationMaxSpeed = 1f;
        [Range(-90f, 90f)] public float VelocityRotationMaxAngle = 45f;
        public Vector3 GrabbedColliderScale = new Vector3(1.5f, 10f, 1.5f);
        
        [ReadOnly] public string CardName;
        [ReadOnly] public bool IsInSocket;

        private TarotCard _card;
        private Rigidbody _rigidbody;
        private TarotDeck _deck;
        private MystiquinnScene _scene;
        private BoxCollider _boxCollider;
        private Transform _bottomPoint;

        private Vector3 _defaultColliderSize;
        private Vector3 _grabbedColliderSize;
        private Vector3 _spawnPosition;
        private Quaternion _deckRotation;
        private bool _currentlyDrawing;
        private bool _firstGrab;
        private bool _seenCard;
        private bool _justSpawned = true;

        ///Initialize this card - called by the Deck when the card is instantiated
        public void Initialize(TarotDeck deck, MystiquinnScene scene)
        {
            _deck = deck;
            _scene = scene;
            _rigidbody = GetComponent<Rigidbody>();
            _card = GetComponent<TarotCard>();
            _boxCollider = GetComponent<BoxCollider>();
            _bottomPoint = transform.Find("BottomPoint");

            _defaultColliderSize = _boxCollider.size;
            _grabbedColliderSize = _defaultColliderSize;
            _grabbedColliderSize.Scale(GrabbedColliderScale);
            
            _deckRotation = ThisTransform.localRotation;
            CardName = _card.Card.GetName();
            gameObject.name = _card.Card.GetName(false);
            _currentlyDrawing = true;
            _firstGrab = true;
            _positionOnTouch = ThisTransform.localPosition;
            _targetPosition = _positionOnTouch;
            _spawnPosition = _positionOnTouch;
            
            OnGrab.AddListener(() =>
            {
                Debug.Log("Card is grabbable: " + IsGrabbable);
                if (!IsGrabbable) return;

                _boxCollider.size = _grabbedColliderSize;
                
                //Innequin should look at the card when grabbed
                IKLookAttention.SupplyTargetObject(transform);

                _scene.RegisterCardGrabbed();
                
                //If the card is sitting on the deck ready to be drawn, we tell the deck to remove it
                //so that we can spawn the next card + begin the animations for this card
                if (_currentlyDrawing) {
                    DrawFromDeck(ThisTransform.position + OffsetOnGrab);
                }
            });
            
            //On release, Innequin returns to their original look mode
            OnRelease.AddListener(() =>
            {
                _boxCollider.size = _defaultColliderSize;
                IKLookAttention.ClearTargetObject();
            });
        }

        ///Removes this card from the deck and spawns a new card on top of the deck to be grabbed next
        public void DrawFromDeck(Vector3 position)
        {
            _positionOnTouch = position;
            _targetPosition = position;
            _spawnPosition = position;
            _currentlyDrawing = false;
            _deck.SpawnFreshCard(true);
        }

        protected override void Update()
        {
            //Cards resting on the ground are grabbable if the deck is available to have cards drawn from it
            if (_rigidbody.useGravity && CurrentSnapTarget == null && _deck != null && _deck.NextCard != null) {
                IsGrabbable = _deck.NextCard.IsGrabbable;
            }
            
            if (HoverEffect) HoverEffect.enabled = IsGrabbable;
            
            if (_deck == null) return;
            
            //initialize basic target position stuff before we begin considering more complex card animations
            if (_justSpawned) {
                base.Update();
                _justSpawned = false;
                return;
            }

            var offsetFromDeck = (IsGrabbed || IsInSocket) ? (_rawInteractionTargetPosition - _deck.transform.position) : Vector3.zero;
            //unsure whether we want this - it means that dragging a card 'up' doesn't remove it from the deck
            //offsetFromDeck.y = 0f;
            var distanceFromDeck = offsetFromDeck.magnitude;

            //When the card is near to the deck, it remains face-down and is magnetized
            //We nede to do some math to project this displacement onto the deck plane
            if (distanceFromDeck < SnapBackDistance) {
                var offsetPos = _targetPosition - _deck.transform.position;
                offsetPos = Vector3.ProjectOnPlane(offsetPos, -_deck.transform.forward);
                offsetPos += _deck.transform.position;
                var localOffset = _deck.NewCardPosition;
                if (IsGrabbed) localOffset += Vector3.back * OffsetOnGrab.y;
                offsetPos += _deck.transform.TransformVector(localOffset);
                _targetPosition = offsetPos;
                _targetRotation = _deckRotation;
            } else {
                //otherwise, we pick the card up and tell Innequin to respond
                if (!_seenCard) {
                    _scene.RespondToCardDraw(CardName);
                    _seenCard = true;
                }

                //ensure we don't clip through the table
                if (IsGrabbed) {
                    var surfaceTransform = _scene.TableSurface;
                    var rawBottomPoint = ThisTransform.TransformVector(_bottomPoint.localPosition) + _rawInteractionTargetPosition;
                    var relativeToSurface = surfaceTransform.InverseTransformPoint(rawBottomPoint);
                    if (relativeToSurface.z < 0.01f && relativeToSurface.x > -0.25f && relativeToSurface.x < 0.25f && relativeToSurface.y > -0.105f && relativeToSurface.y < 0.105f) {
                        var worldSpaceOffset = surfaceTransform.TransformVector(relativeToSurface);
                        _targetPosition = _rawInteractionTargetPosition + Vector3.up * -worldSpaceOffset.y;
                    }
                }
            }

            if (_rigidbody.useGravity) return;
            base.Update();

            //If we're not snapped to the deck, we apply a subtle rotation based on card velocity
            if (distanceFromDeck > SnapBackDistance) {
                _targetRotation = Quaternion.Euler(UprightRotation);
                _targetRotation *= Quaternion.Euler(
                    Mathf.Clamp(_currentVelocity.y / VelocityRotationMaxSpeed, -1f, 1f) * VelocityRotationMaxAngle,
                    0f,
                    Mathf.Clamp(-_currentVelocity.x / VelocityRotationMaxSpeed, -1f, 1f) * VelocityRotationMaxAngle
                );
                if (IsGrabbed) {
                    _scene.TryHighlightActiveBorder(ThisTransform.position);
                }
            }
        }

        protected override void Grab(SpatialPointerState touchData)
        {
            Debug.Log("Card is grabbable: " + IsGrabbable);
            if (!IsGrabbable) return;
            
            //Remove physics from the card on grab
            if (_rigidbody.useGravity) {
                _targetPosition = ThisTransform.position;
            }

            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
            base.Grab(touchData);
        }

        protected override void Release()
        {
            if (!IsGrabbed) return;
            
            base.Release();
            _currentlyDrawing = false;

            //Add physics back to the card on release
            if (CurrentSnapTarget == null) {
                _rigidbody.isKinematic = false;
                _rigidbody.useGravity = true;
                _rigidbody.velocity = _currentVelocity;
                
                //For a short time after release, we check to see if the card was dropped onto a slot
                StartCoroutine(TryToSnapOnLanding());
                return;
            }

            //If the card has been dropped onto the deck, we shrink and remove it (adding it back to the deck)
            var snappedDeck = CurrentSnapTarget.GetComponent<TarotDeck>();
            if (snappedDeck != null && _firstGrab) {
                _scene.RegisterCardReleased();
                IsGrabbable = false;
                //wait a moment for it to snap back into the deck, then remove the gameobject
                StartCoroutine(DestroySelf());
                return;
            }

            //the only other snap targets are the card slots, so we we tell Innequin about it!
            var slot = CurrentSnapTarget.GetComponent<TarotSpreadSocket>();
            IsInSocket = true;
            _scene.RespondToCardPlaced(CardName, slot.SocketName);

            IsGrabbable = false;
        }

        private IEnumerator TryToSnapOnLanding()
        {
            var endCheckTime = Time.time + 1f;
            while (Time.time < endCheckTime) {
                if (_rigidbody.IsSleeping()) {
                    break;
                }

                yield return null;
            }

            TarotSpreadSocket chosenSocket = null;
            foreach (var socket in _scene.Sockets) {
                if (socket.SnappedObjects.Count > 0) continue;
                var distance = (ThisTransform.position - socket.MagnetPosition).magnitude;
                if (distance < socket.MagneticDistance * 0.5f) {
                    chosenSocket = socket;
                    break;
                }
            }

            if (chosenSocket != null) {
                SnapToTarget(chosenSocket);
                IsInSocket = true;
                _scene.RespondToCardPlaced(CardName, chosenSocket.SocketName);
                _rigidbody.isKinematic = true;
                _rigidbody.useGravity = false;
                IsGrabbable = false;
            } else {
                //if we were drawing this card, set the deck glow active again to encourage the user
                //to draw another card
                _scene.RegisterCardReleased();
                
                //Once dropped, cards can no longer be grabbed, and shrink down to nothing after a short delay
                //StartCoroutine(DestroySelf(1.5f, 0.001f, 2f));
            }
        }

        //Shrinks the card and removes it
        private IEnumerator DestroySelf(float downscaleTime = 0.5f, float downscaleSize = 0.3f, float delay = 0f)
        {
            IsGrabbable = false;
            if (delay > 0f) yield return new WaitForSeconds(delay);
            var startScale = ThisTransform.localScale;
            for (var i = 0f; i < 1f; i += Time.deltaTime / downscaleTime) {
                ThisTransform.localScale = Vector3.Lerp(startScale, startScale * downscaleSize, i);
                yield return null;
            }

            _deck.AddCardToDeck(_card.Card.GetId);
            Destroy(gameObject);
        }

#if UNITY_EDITOR
        [EditorButton]
        public void SendPickupTrigger()
        {
            _scene.RespondToCardDraw(CardName);
        }
        
        [EditorButton]
        public void SendPlacedPast()
        {
            _scene.RespondToCardPlaced(CardName, "Past");
        }

        [EditorButton]
        public void SendPlacedPresent()
        {
            _scene.RespondToCardPlaced(CardName, "Present");
        }

        [EditorButton]
        public void SendPlacedFuture()
        {
            _scene.RespondToCardPlaced(CardName, "Future");
        }
#endif
    }
}