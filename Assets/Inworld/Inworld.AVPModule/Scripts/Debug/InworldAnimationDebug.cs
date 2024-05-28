using Inworld.Assets;
using UnityEngine;

namespace LC.DebugUtils
{
    public class InworldAnimationDebug : MonoBehaviour
    {
        public bool UseRawValues = true;

        [Header("Raw Values")]
        public float RawRemainSec;

        public float RawRandom;
        public int RawGesture;
        public int RawEmotion;
        public int RawMainStatus;

        [Header("Emotion Values")]
        public AnimMainStatus MainStatus = AnimMainStatus.Neutral;

        public Emotion Emotion = Emotion.Neutral;
        public Gesture Gesture = Gesture.Neutral;

        private Animator _animator;

        public void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Update()
        {
            if (UseRawValues) {
                _animator.SetFloat("RemainSec", RawRemainSec);
                _animator.SetFloat("Random", RawRandom);
                _animator.SetInteger("Gesture", RawGesture);
                _animator.SetInteger("Emotion", RawEmotion);
                _animator.SetInteger("MainStatus", RawMainStatus);
            } else {
                _animator.SetFloat("RemainSec", RawRemainSec);
                _animator.SetFloat("Random", RawRandom);
                _animator.SetInteger("Gesture", (int) MainStatus);
                _animator.SetInteger("Emotion", (int) Emotion);
                _animator.SetInteger("MainStatus", (int) Gesture);
            }
        }

        public void RandomizeEmotion()
        {
            Emotion = (Emotion) Random.Range(0, 5);
        }

        public void RandomizeGesture()
        {
            Gesture = (Gesture) Random.Range(0, 24);
        }
    }
}