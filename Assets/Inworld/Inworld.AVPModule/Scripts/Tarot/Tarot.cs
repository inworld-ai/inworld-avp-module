using System;
using UnityEngine;

namespace LC.Tarot
{
    /// The tarot suit. Indexed on hundreds to make generating IDs easier
    /// i.e. Suit + Rank = ID (e.g. 208 = eight of cups)
    public enum Suit
    {
        Major = 0,
        Coins = 100,
        Cups = 200,
        Swords = 300,
        Wands = 400,
    }

    /// Tarot card ranks. Not used for Major Arcana
    public enum Rank
    {
        None = 0,
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Page = 11,
        Knight = 12,
        Queen = 13,
        King = 14,
    }

    /// The 22 Major Arcana
    public enum MajorArcana
    {
        None = 0,
        TheFool = 1,
        TheMagician = 2,
        TheHighPriestess = 3,
        TheEmpress = 4,
        TheEmperor = 5,
        TheHierophant = 6,
        TheLovers = 7,
        TheChariot = 8,
        Strength = 9,
        TheHermit = 10,
        WheelOfFortune = 11,
        Justice = 12,
        TheHangedMan = 13,
        Death = 14,
        Temperance = 15,
        TheDevil = 16,
        TheTower = 17,
        TheStar = 18,
        TheMoon = 19,
        TheSun = 20,
        Judgement = 21,
        TheWorld = 22,
    }
    
    /// Utility class for managing tarot cards.
    [Serializable]
    public struct Card
    {
        public Suit Suit;
        public Rank Rank;
        public MajorArcana Major;

        /// Gets a unique ID for this suit + rank/major-arcana combination
        public int GetId => (int) Suit + (Suit == Suit.Major ? (int) Major : (int) Rank);

        /// Parses a suit + rank/major-arcana from an integer ID
        public static Card GetCardFromId(int id)
        {
            var suitId = 100 * Mathf.FloorToInt(id / 100f);
            if (suitId < 0 || suitId > 500) throw new FormatException("Invalid ID, got suit ID of " + suitId);
            var suit = (Suit) suitId;
            
            var rankMajorId = id % 100;
            if (rankMajorId <= 0 || suit == Suit.Major && rankMajorId > 22 || suit != Suit.Major && rankMajorId > 14) {
                throw new FormatException("Invalid ID, got rank / major arcana ID of " + rankMajorId);
            }
            var major = suit == Suit.Major ? (MajorArcana) rankMajorId : MajorArcana.None;
            var rank = suit == Suit.Major ? Rank.None : (Rank) rankMajorId;
            
            return new Card {
                Suit = suit,
                Major = major,
                Rank = rank
            };
        }

        /// Gets a pretty name for the card
        public string GetName(bool withSpaces = true)
        {
            if (Suit == Suit.Major) {
                if (!withSpaces) return Major.ToString();
                
                var name = "";
                var rawName = Major.ToString();
                for (var i = 0; i < rawName.Length; i++) {
                    if (rawName[i].ToString().ToLower() != rawName[i].ToString() && i != 0) {
                        name += " " + rawName[i];
                    } else name += rawName[i];
                }

                return name;
            }

            return withSpaces 
                ? "The " + Rank + " of " + Suit
                : Rank + "Of" + Suit;
        }
    }
}