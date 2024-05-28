using System.Collections;
using UnityEngine;

namespace LC.GameShow
{
    public class LedFlasher : MonoBehaviour
    {
        public Color DingdingColor = Color.green;
        public Color WahwahColor = Color.red;
        public float FlashTime = 0.7f;

        public Renderer GridRenderer;
        public Renderer FloorRenderer;

        private Material _gridMaterial;
        private Material _floorMaterial;
        private Color _floorWhite = new Color(0.5f, 0.5f, 0.5f);
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public void FlashDingding()
        {
            StopAllCoroutines();
            StartCoroutine(FlashRoutine(DingdingColor));
        }

        public void FlashWahwah()
        {
            StopAllCoroutines();
            StartCoroutine(FlashRoutine(WahwahColor));
        }

        private IEnumerator FlashRoutine(Color targetColor)
        {
            if (_gridMaterial == null) {
                _gridMaterial = Application.isEditor ? GridRenderer.material : GridRenderer.sharedMaterial;
                _floorMaterial = Application.isEditor ? FloorRenderer.material : FloorRenderer.sharedMaterial;
            }

            for (var i = 0f; i < 1f; i += Time.deltaTime / FlashTime) {
                _gridMaterial.SetColor(EmissionColor, Color.Lerp(targetColor, Color.white, i));
                yield return null;
                _floorMaterial.SetColor(EmissionColor, Color.Lerp(targetColor, _floorWhite, i));
            }

            _gridMaterial.SetColor(EmissionColor, Color.white);
            _floorMaterial.SetColor(EmissionColor, _floorWhite);
        }
    }
}