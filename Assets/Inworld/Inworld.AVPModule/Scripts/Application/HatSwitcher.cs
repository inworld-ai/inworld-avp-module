using System.Collections;
using System.Collections.Generic;
using Inworld.UI;
using Lunity;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LC
{
    /// Manages the loading and unloading of separate Innequin scenes
    public class HatSwitcher : MonoBehaviour
    {
        public enum Hat
        {
            None = 0,
            GameShow = 1,
            Artefact = 2,
            Fortune = 3,
        }

        public Hat CurrentHat = Hat.None;

        private Dictionary<Hat, string> _sceneNames;
        private bool _animating;

        public void Awake()
        {
            _sceneNames = new Dictionary<Hat, string>();
            _sceneNames.Add(Hat.None, "1_Home");
            _sceneNames.Add(Hat.GameShow, "2_GameShow");
            _sceneNames.Add(Hat.Artefact, "3_Artefact");
            _sceneNames.Add(Hat.Fortune, "4_Fortune");
        }

        public void SetHat(Hat newHat)
        {
            if (newHat == CurrentHat) return;
            if (_animating) return;
            StartCoroutine(SetHatRoutine(newHat));
        }

        public void SetHat(int hatID)
        {
            SetHat((Hat) hatID);
        }

        private IEnumerator SetHatRoutine(Hat newHat)
        {
            _animating = true;
            yield return new WaitForSeconds(0.5f);

            //Unload the existing scene
            var oldHat = CurrentHat;
            var unloadOp = SceneManager.UnloadSceneAsync(_sceneNames[oldHat], UnloadSceneOptions.None);
            while (!unloadOp.isDone) yield return null;

            CurrentHat = newHat;

            //Load the new scene
            var loadOp = SceneManager.LoadSceneAsync(_sceneNames[CurrentHat], LoadSceneMode.Additive);
            while (!loadOp.isDone) yield return null;

            //Wait for Inworld to fully initialize with the new character
            var scene = FindObjectOfType<GdcScene>();
            while (scene.IsLoading) yield return null;

            _animating = false;
        }

#if UNITY_EDITOR
        [EditorButton] public void SetHatNone()
        {
            SetHat(Hat.None);
        }

        [EditorButton] public void SetHatGameShow()
        {
            SetHat(Hat.GameShow);
        }

        [EditorButton] public void SetHatArtefact()
        {
            SetHat(Hat.Artefact);
        }

        [EditorButton] public void SetHatFortune()
        {
            SetHat(Hat.Fortune);
        }
#endif
    }
}