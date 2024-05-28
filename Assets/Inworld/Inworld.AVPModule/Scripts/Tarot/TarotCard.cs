using UnityEngine;

namespace LC.Tarot
{
    ///Manages the material of a card
    [ExecuteAlways]
    public class TarotCard : MonoBehaviour
    {
        public Card Card;
        public TarotCardTextures Textures;

        private Material _material;
        private int _setMaterialId;

        private void GetMaterial()
        {
            var r = GetComponent<Renderer>();
            _material = Application.isPlaying ? r.materials[1] : r.sharedMaterials[1];
        }

        public void Update()
        {
            var cardId = Card.GetId;
            if (cardId != _setMaterialId) {
                SetMaterial(cardId);
            }
        }

        private void SetMaterial(int id)
        {
            if (_material == null) GetMaterial();
            Textures.GetTextureSetup(id, out var albedo, out var metallic, out var normal, out var offset);
            _material.SetTexture("_MainTex", albedo);
            _material.SetTexture("_MetallicGlossMap", metallic);
            _material.SetTexture("_BumpMap", normal);

            _material.SetTextureOffset("_MainTex", offset);
        }
    }
}