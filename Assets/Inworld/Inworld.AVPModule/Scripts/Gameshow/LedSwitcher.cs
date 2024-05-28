using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LC.GameShow
{
    public class LedSwitcher : MonoBehaviour
    {
        public List<Texture> GridImages = new List<Texture>();
        public List<Texture> GlowImages = new List<Texture>();
        public List<Texture> FloorImages = new List<Texture>();
        [Space(10f)]
        public Renderer GridRenderer;
        public Renderer GlowRenderer;
        public Renderer GlowRendererBack;
        public Renderer FloorRenderer;
        public Renderer PreloadRenderer;

        private Material _gridMat;
        private Material _glowMat;
        private Material _glowMatB;
        private Material _floorMat;

        public void Awake()
        {
            _gridMat = Application.isEditor ? GridRenderer.material : GridRenderer.sharedMaterial;
            _glowMat = Application.isEditor ? GlowRenderer.material : GlowRenderer.sharedMaterial;
            _glowMatB = Application.isEditor ? GlowRendererBack.material : GlowRendererBack.sharedMaterial;
            _floorMat = Application.isEditor ? FloorRenderer.material : FloorRenderer.sharedMaterial;

            StartCoroutine(PreloadTextures());
        }

        //Ensure we don't have any one-frame magenta flickers as the first spin occurs
        private IEnumerator PreloadTextures()
        {
            var mat = PreloadRenderer.material;
            foreach (var tex in GridImages) {
                mat.mainTexture = tex;
                yield return null;
            }
            foreach (var tex in GlowImages) {
                mat.mainTexture = tex;
                yield return null;
            }
            foreach (var tex in FloorImages) {
                mat.mainTexture = tex;
                yield return null;
            }

            Destroy(PreloadRenderer.material);
            Destroy(PreloadRenderer.gameObject);
        }

        public void CategorySwitch(int i)
        {
            _gridMat.mainTexture = GridImages[i];
            _gridMat.SetTexture("_EmissionMap", GridImages[i]);

            _glowMat.mainTexture = GlowImages[i];
            _glowMatB.mainTexture = GlowImages[i];

            _floorMat.mainTexture = FloorImages[i];
            _floorMat.SetTexture("_EmissionMap", FloorImages[i]);

        }
    }
}