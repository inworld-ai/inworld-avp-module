using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LC.Tarot
{
    /// General-purpose utility class for dealing with decks of cards (and similar stacks of things)
    [Serializable]
    public class GameDeckBase<T>
    {
        [SerializeField]
        private List<T> _items;

        public List<T> Items {
            get {
                if (_items == null) _items = new List<T>();
                return _items;
            }
            protected set => _items = value;
        }

        public Action<List<T>> OnItemsChanged;
        public int Count => Items?.Count ?? 0;

        /// Provides a new list of items to the deck
        public virtual void SetItems(IEnumerable<T> items)
        {
            _items = items.ToList();
            OnItemsChanged?.Invoke(Items);
        }

        /// Performs a Fisher-Yates shuffle on the deck, randomizing the order of the cards it contains
        public void Shuffle()
        {
            if (Items.Count == 0) return;
            Shuffles.FisherYates(ref _items);
            OnItemsChanged?.Invoke(Items);
        }

        /// Removes the top card from the deck
        public virtual T TakeFromTop()
        {
            if (Items.Count == 0) return default;

            var topCard = Items[0];
            Items.RemoveAt(0);
            OnItemsChanged?.Invoke(Items);
            return topCard;
        }

        /// Peek at the top card of the deck without removing it
        public virtual T PeekTop()
        {
            return Items.Count == 0
                ? default
                : Items[0];
        }

        /// Removes the bottom card from the deck
        public virtual T TakeFromBottom()
        {
            if (Items.Count == 0) return default;

            var bottomCard = Items[Items.Count - 1];
            Items.RemoveAt(Items.Count - 1);
            OnItemsChanged?.Invoke(Items);
            return bottomCard;
        }

        /// Peek at the bottom card of the deck without removing it
        public virtual T PeekBottom()
        {
            return Items.Count == 0
                ? default
                : Items[Items.Count - 1];
        }

        /// Removes a random card from the deck
        public virtual T TakeRandom()
        {
            if (Items.Count == 0) return default;

            var choice = Random.Range(0, Items.Count);
            var topCard = Items[choice];
            Items.RemoveAt(choice);
            OnItemsChanged?.Invoke(Items);
            return topCard;
        }

        /// Peek at a random card of the deck without removing it
        public virtual T PeekRandom()
        {
            return Items.Count == 0
                ? default
                : Items[Random.Range(0, Items.Count)];
        }

        /// Removes an item from the deck at the specified position
        public virtual T RemoveItem(int index)
        {
            if (index < 0 || index >= Items.Count) {
                Debug.LogWarning("Couldn't remove item - outside of range!");
                return default;
            }

            var item = Items[index];
            Items.RemoveAt(index);
            OnItemsChanged?.Invoke(Items);
            return item;
        }

        /// Explicitly sets the item at the specified position to a new value - use with caution!
        public virtual T SetItem(int index, T item)
        {
            if (index < 0 || index >= Items.Count) {
                Debug.LogWarning("Couldn't set item - outside of range!");
                return default;
            }

            Items[index] = item;
            OnItemsChanged?.Invoke(Items);
            return Items[index];
        }

        /// Adds a new item to the top of the deck (position 0)
        public virtual T AddItemToTop(T item)
        {
            Items.Insert(0, item);
            OnItemsChanged?.Invoke(Items);
            return Items[0];
        }

        /// Adds a new item to the bottom of the deck (position count - 1)
        public virtual T AddItemToBottom(T item)
        {
            Items.Add(item);
            OnItemsChanged?.Invoke(Items);
            return Items[Items.Count - 1];
        }

        /// Adds a new item to a random location within the deck
        public virtual T AddItemRandom(T item)
        {
            var newPos = Random.Range(0, Items.Count);
            Items.Insert(newPos, item);
            OnItemsChanged?.Invoke(Items);
            return Items[newPos];
        }
    }
}