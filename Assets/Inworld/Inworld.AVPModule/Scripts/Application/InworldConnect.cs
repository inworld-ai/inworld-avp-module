using System;
using System.Collections;
using System.Collections.Generic;
using Inworld;
using LC.Interaction;
using LC.InworldUtils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LC
{
    public class InworldConnect : MonoBehaviour
    {
        public enum SceneToLoadOption
        {
            None,
            Home,
            GameShow,
            Artefact,
            Fortune,
        }

        /// Which scene to load by default - mainly just for debugging
        public SceneToLoadOption SceneToLoad = SceneToLoadOption.Home;
        
        [Header("Scene References")]
        public TMP_Text StatusText;
        public CanvasGroup MainInputCanvas;
        public GameObject[] EnableAfterConnect;
        public GameObject[] DisableAfterConnect;

        private FaceSpritePreloader _spritePreloader;

        public void Start()
        {
            _spritePreloader = GetComponentInChildren<FaceSpritePreloader>();
            StartCoroutine(ConnectRoutine());
        }

        private IEnumerator ConnectRoutine()
        {
            //Initialize the sprites of the face, to prevent VisionOS 'missing texture' bugs due to the sprites
            //not being preloaded at first
            StatusText.text = "Initializing sprites...";
            var minMicWaitTime = 1f;
            var startTime = Time.time;
            yield return StartCoroutine(_spritePreloader.RunthroughSprites());
            
            //We also want to wait at least two seconds to ensure the VisionOS audio session is initialized, so we
            //kill two birds with one stone
            StatusText.text = "Initializing microphone...";
            VisionOsMicEnabler.MicAccessCallbackResult result = VisionOsMicEnabler.MicAccessCallbackResult.Success;
            yield return VisionOsMicEnabler.GetMicAccess(r => result = r, 100);
            switch (result) {
                case VisionOsMicEnabler.MicAccessCallbackResult.Success:
                    while (Time.time - startTime < minMicWaitTime) yield return null;
                    break;
                case VisionOsMicEnabler.MicAccessCallbackResult.NoDevicesAvailable:
                    StatusText.text = "No microphone found!";
                    yield return new WaitForSeconds(1f);
                    break;
                case VisionOsMicEnabler.MicAccessCallbackResult.FailedToStartMicrophone:
                    StatusText.text = "Failed to start microphone!";
                    yield return new WaitForSeconds(1f);
                    break;
                case VisionOsMicEnabler.MicAccessCallbackResult.NoAudioDetected:
                    StatusText.text = "No audio data detected!";
                    yield return new WaitForSeconds(1f);
                    break;
            }

            //Load the main scene
            StatusText.text = "Loading...";
            var op = SceneManager.LoadSceneAsync(GetSceneName(SceneToLoad), LoadSceneMode.Additive);
            while (!op.isDone) yield return null;
            StatusText.text = "Done!";
            
            //We wait for an extra moment to ensure everything is 'settled' before beginning the main application UX
            yield return new WaitForSeconds(0.5f);
            foreach (var obj in EnableAfterConnect) obj.SetActive(true);
            foreach (var obj in DisableAfterConnect) obj.SetActive(false);

            for (var i = 0f; i < 1f; i += Time.deltaTime / 0.5f) {
                MainInputCanvas.alpha = i;
                yield return null;
            }
        }

        private string GetSceneName(SceneToLoadOption scene)
        {
            switch (scene) {
                case SceneToLoadOption.None:
                    return "";
                case SceneToLoadOption.Home:
                    return "1_Home";
                case SceneToLoadOption.GameShow:
                    return "2_GameShow";
                case SceneToLoadOption.Artefact:
                    return "3_Artefact";
                case SceneToLoadOption.Fortune:
                    return "4_Fortune";
                default:
                    Debug.LogError("Unknown scene type " + scene + " - not loading anything");
                    return "";
            }
        }
    }
}