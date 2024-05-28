using System;
using System.Collections;
using LC.Tarot;
using Lunity;
using UnityEngine;
using UnityEngine.Events;

namespace LC.GameShow
{
    public class CategorySpinner : MonoBehaviour
    {
        [System.Serializable] public class UnityIntEvent : UnityEvent<int> { }
        [System.Serializable] public class UnityFloatEvent : UnityEvent<float> { }

        [Tooltip("How many categories are possible")]
        public int CategoryCount = 6;
        [Tooltip("How long the spin lasts - note that it might be a touch longer than this")]
        public float SpinDuration = 10f;
        [Tooltip("Maps spin progress (0 - 1) to the flicker interval in seconds")]
        public AnimationCurve IntervalCurve = AnimationCurve.Linear(0f, 0.1f, 1f, 1f);

        public bool CanSpin = false;
        
        public Action<int> OnSpinStart;
        public UnityIntEvent OnSpinCategoryFlicker;
        public UnityFloatEvent OnSpinProgressFlicker;
        
        public Action<int> OnSpinEnd;
        
        [ReadOnly] public bool IsSpinning;

        private GameDeckBase<int> _categoryDeck;

        public void Awake()
        {
            _categoryDeck = new GameDeckBase<int>();
            for (var i = 0; i < CategoryCount; i++) {
                _categoryDeck.AddItemToTop(i);
            }
            _categoryDeck.Shuffle();
        }


        /// Starts spinning categories, raising flicker events
        [EditorButton]
        public void TriggerSpin()
        {
            if (IsSpinning) return;
            if (!CanSpin) return;
            StartCoroutine(SpinRoutine());
        }

        private IEnumerator SpinRoutine()
        {
            var finalCategory = _categoryDeck.PeekTop();
            OnSpinStart?.Invoke(finalCategory);
            IsSpinning = true;

            var shuffleCountdown = _categoryDeck.Count;
            var time = 0f;
            while (time < SpinDuration) {
                var flickerCategory = _categoryDeck.TakeFromTop();
                OnSpinCategoryFlicker?.Invoke(flickerCategory);
                OnSpinProgressFlicker?.Invoke(time / SpinDuration);
                _categoryDeck.AddItemToBottom(flickerCategory);
                shuffleCountdown--;
                
                //after we've shown all six categories, shuffle the deck and make sure we don't show the same one twice in a row
                if (shuffleCountdown <= 0) {
                    _categoryDeck.Shuffle();
                    if (_categoryDeck.PeekTop() == flickerCategory) {
                        _categoryDeck.AddItemToBottom(_categoryDeck.TakeFromTop());
                    }
                }
                
                var delay = IntervalCurve.Evaluate(time / SpinDuration);
                yield return new WaitForSeconds(delay);
                time += delay;
            }
            _categoryDeck.Shuffle();
            OnSpinCategoryFlicker?.Invoke(finalCategory);
            OnSpinProgressFlicker?.Invoke(1f);
            OnSpinEnd?.Invoke(finalCategory);
            IsSpinning = false;
        }

        public void SetCanSpin(bool canSpin)
        {
            CanSpin = canSpin;
        }
    }
}