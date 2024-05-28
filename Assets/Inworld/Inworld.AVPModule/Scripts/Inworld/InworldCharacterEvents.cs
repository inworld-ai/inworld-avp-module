using System;
using Inworld;
using Inworld.Packet;

namespace LC.InworldUtils
{
    /// A class that splits out the OnCharacterInteraction into separate events for each packet type
    /// Simply for convenience!
    /// Will still work even if we switch InworldControllers (although existing event listeners will no longer fire)
    public class InworldCharacterEvents
    {
        private static InworldController _eventController;

        public static Action OnInworldControllerChanged;
        
        private static Action<ActionPacket> _onAction;
        public static Action<ActionPacket> OnAction {
            get {
                TryAddCharacterInteractionListener();
                return _onAction;
            }
            set {
                TryAddCharacterInteractionListener();
                _onAction = value;
            }
        }
        
        private static Action<AudioPacket> _onAudio;
        public static Action<AudioPacket> OnAudio {
            get {
                TryAddCharacterInteractionListener();
                return _onAudio;
            }
            set {
                TryAddCharacterInteractionListener();
                _onAudio = value;
            }
        }
        
        private static Action<ControlPacket> _onControl;
        public static Action<ControlPacket> OnControl {
            get {
                TryAddCharacterInteractionListener();
                return _onControl;
            }
            set {
                TryAddCharacterInteractionListener();
                _onControl = value;
            }
        }
        
        private static Action<CustomPacket> _onCustom;
        public static Action<CustomPacket> OnCustom {
            get {
                TryAddCharacterInteractionListener();
                return _onCustom;
            }
            set {
                TryAddCharacterInteractionListener();
                _onCustom = value;
            }
        }
        
        private static Action<EmotionPacket> _onEmotion;
        public static Action<EmotionPacket> OnEmotion {
            get {
                TryAddCharacterInteractionListener();
                return _onEmotion;
            }
            set {
                TryAddCharacterInteractionListener();
                _onEmotion = value;
            }
        }
        
        private static Action<GesturePacket> _onGesture;
        public static Action<GesturePacket> OnGesture {
            get {
                TryAddCharacterInteractionListener();
                return _onGesture;
            }
            set {
                TryAddCharacterInteractionListener();
                _onGesture = value;
            }
        }
        
        private static Action<InworldNetworkPacket> _onInworldNetwork;
        public static Action<InworldNetworkPacket> OnInworldNetwork {
            get {
                TryAddCharacterInteractionListener();
                return _onInworldNetwork;
            }
            set {
                TryAddCharacterInteractionListener();
                _onInworldNetwork = value;
            }
        }
        
        private static Action<MutationPacket> _onMutation;
        public static Action<MutationPacket> OnMutation {
            get {
                TryAddCharacterInteractionListener();
                return _onMutation;
            }
            set {
                TryAddCharacterInteractionListener();
                _onMutation = value;
            }
        }
        
        private static Action<RelationPacket> _onRelation;
        public static Action<RelationPacket> OnRelation {
            get {
                TryAddCharacterInteractionListener();
                return _onRelation;
            }
            set {
                TryAddCharacterInteractionListener();
                _onRelation = value;
            }
        }
        
        private static Action<TextPacket> _onText;
        public static Action<TextPacket> OnText {
            get {
                TryAddCharacterInteractionListener();
                return _onText;
            }
            set {
                TryAddCharacterInteractionListener();
                _onText = value;
            }
        }

        private static void TryAddCharacterInteractionListener()
        {
            if (_eventController != null) return;
            _onCustom = null;
            _eventController = InworldController.Instance;
            _eventController.OnCharacterInteraction += HandleCharacterInteraction;
            OnInworldControllerChanged?.Invoke();
        }

        private static void HandleCharacterInteraction(InworldPacket packet)
        {
            if (packet is ActionPacket actionPacket) _onAction?.Invoke(actionPacket);
            if (packet is AudioPacket audioPacket) _onAudio?.Invoke(audioPacket);
            if (packet is ControlPacket controlPacket) _onControl?.Invoke(controlPacket);
            if (packet is CustomPacket customPacket) _onCustom?.Invoke(customPacket);
            if (packet is EmotionPacket emotionPacket) _onEmotion?.Invoke(emotionPacket);
            if (packet is GesturePacket gesturePacket) _onGesture?.Invoke(gesturePacket);
            if (packet is InworldNetworkPacket inworldNetworkPacket) _onInworldNetwork?.Invoke(inworldNetworkPacket);
            if (packet is MutationPacket mutationPacket) _onMutation?.Invoke(mutationPacket);
            if (packet is RelationPacket relationPacket) _onRelation?.Invoke(relationPacket);
            if (packet is TextPacket textPacket) _onText?.Invoke(textPacket);
            
        }
    }
}