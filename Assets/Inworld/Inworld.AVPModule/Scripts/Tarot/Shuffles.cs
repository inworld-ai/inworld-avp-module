using System.Collections.Generic;
using UnityEngine;

namespace LC.Tarot
{
    public static class Shuffles
    {
        /// <summary>
        ///     Performs a Fisher-Yates shuffle on the deck, randomizing the order of the cards it contains
        /// </summary>
        public static void FisherYates<T>(ref T[] items)
        {
            if (items.Length == 0) return;
            for (var i = items.Length - 1; i >= 1; i--) {
                var j = Random.Range(0, i + 1);
                var cardA = items[i];
                var cardB = items[j];
                items[i] = cardB;
                items[j] = cardA;
            }
        }

        /// <summary>
        ///     Performs a Fisher-Yates shuffle on the deck, randomizing the order of the cards it contains
        /// </summary>
        public static void FisherYates<T>(ref List<T> items)
        {
            if (items.Count == 0) return;
            for (var i = items.Count - 1; i >= 1; i--) {
                var j = Random.Range(0, i + 1);
                var cardA = items[i];
                var cardB = items[j];
                items[i] = cardB;
                items[j] = cardA;
            }
        }
    }
}