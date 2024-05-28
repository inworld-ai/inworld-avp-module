using System;
using UnityEngine;

namespace LC.Tarot
{
    [CreateAssetMenu(fileName = "TarotCardTextures", menuName = "TarotCardTextures", order = 100)]
    public class TarotCardTextures : ScriptableObject
    {
        public float[] Rows = {
            0.756f, 0.511f, 0.266f, 0.018f
        };

        [Space(10f)]
        public Texture2D SwordsCoinsAlbedo;
        public Texture2D SwordsCoinsMetallic;
        public Texture2D SwordsCoinsNormal;

        [Space(10f)]
        public Texture2D WandsCupsAlbedo;
        public Texture2D WandsCupsMetallic;
        public Texture2D WandsCupsNormal;

        [Space(10f)]
        public Texture2D MajorArcanaAlbedo;
        public Texture2D MajorArcanaMetallic;
        public Texture2D MajorArcanaNormal;

        ///Takes in a card ID and returns the textures and texture offset required to properly configure the card's material
        public void GetTextureSetup(int id, out Texture2D albedo, out Texture2D metallic, out Texture2D normal,
            out Vector2 offset)
        {
            var suit = (Suit) (id - id % 100);
            var rank = suit == Suit.Major ? 0 : (Rank) (id % 100);
            var major = suit == Suit.Major ? (MajorArcana) (id % 100) : 0;
            if (rank == Rank.None && major == MajorArcana.None) {
                throw new UnityException("Provided ID of " + id + " has no rank or major arcana!");
            }

            var majorIndex = (int) major - 1;
            var suitIndex = (int) rank - 1;
            if (suit == Suit.Coins || suit == Suit.Cups) suitIndex += 14;

            switch (suit) {
                case Suit.Major:
                    albedo = MajorArcanaAlbedo;
                    metallic = MajorArcanaMetallic;
                    normal = MajorArcanaNormal;
                    offset = new Vector2(majorIndex % 7 / 7f, Rows[Mathf.FloorToInt(majorIndex / 7f)]);
                    break;
                case Suit.Coins:
                case Suit.Swords:
                    albedo = SwordsCoinsAlbedo;
                    metallic = SwordsCoinsMetallic;
                    normal = SwordsCoinsNormal;
                    offset = new Vector2(suitIndex % 7 / 7f, Rows[Mathf.FloorToInt(suitIndex / 7f)]);
                    break;
                case Suit.Wands:
                case Suit.Cups:
                    albedo = WandsCupsAlbedo;
                    metallic = WandsCupsMetallic;
                    normal = WandsCupsNormal;
                    offset = new Vector2(suitIndex % 7 / 7f, Rows[Mathf.FloorToInt(suitIndex / 7f)]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("No suit found for ID " + id);
            }

            //note - in Polyspatial 1.0.x, texture offset y values are flipped. Fixed in Polyspatial 1.1.1
#if !UNITY_EDITOR && UNITY_VISIONOS
            //offset.y = 1f - offset.y;
#endif
        }
    }
}