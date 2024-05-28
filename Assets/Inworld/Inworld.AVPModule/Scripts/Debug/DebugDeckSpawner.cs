using LC.Tarot;
using Lunity;
using UnityEngine;

namespace LC.DebugUtils
{
    /// Spawns all available tarot cards in the scene, to debug material configuration and texture appearance
    public class DebugDeckSpawner : MonoBehaviour
    {
        public GameObject Template;

        public float Scale = 1.7f;
        public Vector2 Size = new Vector2(1f, 1f);

        [EditorButton]
        public void Place()
        {
            for (var i = transform.childCount - 1; i >= 0; i--) {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            for (var i = 0; i < 500; i += 100) {
                if (i == 0) {
                    //major arcana
                    for (var j = 1; j <= 11; j++) {
                        var pos = new Vector3(
                            Mathf.Lerp(-Size.x * 0.5f, Size.x * 0.5f, Mathf.InverseLerp(1f, 14f, (j + 1.5f))),
                            Mathf.Lerp(Size.y * 0.5f, -Size.y * 0.5f, 1f / 6f),
                            0f);
                        SpawnCard(i + j, pos);
                    }

                    for (var j = 12; j <= 22; j++) {
                        var pos = new Vector3(
                            Mathf.Lerp(-Size.x * 0.5f, Size.x * 0.5f, Mathf.InverseLerp(1f, 14f, j - 9.5f)),
                            Mathf.Lerp(Size.y * 0.5f, -Size.y * 0.5f, 2f / 6f),
                            0f);
                        SpawnCard(i + j, pos);
                    }
                } else {
                    var y = Mathf.Lerp(Size.y * 0.5f, -Size.y * 0.5f, (i + 200f) / 100f / 6f);
                    for (var j = 1; j <= 14; j++) {
                        var pos = new Vector3(
                            Mathf.Lerp(-Size.x * 0.5f, Size.x * 0.5f, Mathf.InverseLerp(1f, 14f, j)),
                            y,
                            0f);
                        SpawnCard(i + j, pos);
                    }
                }
            }
        }

        private void SpawnCard(int id, Vector3 localPosition)
        {
            var newCard = Instantiate(Template).GetComponent<TarotCard>();
            newCard.transform.parent = transform;
            newCard.transform.localPosition = localPosition;
            newCard.transform.localScale = Vector3.one * Scale;
            newCard.transform.Rotate(-90f, 0f, 0f, Space.Self);
            newCard.Card = Tarot.Card.GetCardFromId(id);
        }
    }
}