using System;
using System.Collections;
using System.Collections.Generic;
using Inworld;
using UnityEngine;

namespace LC.InworldUtils
{
    /// A set of coroutine-based utilties for managing various Inworld interactions and methods
    public class InworldAsyncUtils : MonoBehaviour
    {
        
        private static InworldAsyncUtils _instance;
        
        /// Singleton instance of the utils class.
        /// Will create an instance and put it in DontDestroyOnLoad if it doesn't already exist.
        public static InworldAsyncUtils Instance {
            get {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<InworldAsyncUtils>();
                if (_instance == null) {
                    var newObj = new GameObject("InworldAsyncUtils");
                    _instance = newObj.AddComponent<InworldAsyncUtils>();
                    DontDestroyOnLoad(newObj);
                }

                return _instance;
            }
        }

        /// Loads the specified character - must be present in the currently active scene
        public static Coroutine LoadCharacter(InworldCharacter character, bool autoRecord = true)
        {
            var coroutine = Instance.StartCoroutine(Instance.LoadCharacterInstance(character, autoRecord));
            return coroutine;
        }

        /// Initializes the Inworld controller - doesn't need to be called if Auto Start is enabled in its inspector
        public static Coroutine InitializeController()
        {
            var coroutine = Instance.StartCoroutine(Instance.InitializeControllerInstance());
            return coroutine;
        }

        /// Loads the scene preconfigured in the Inworld controller
        public static Coroutine LoadScene()
        {
            var coroutine = Instance.StartCoroutine(Instance.LoadSceneInstance());
            return coroutine;
        }

        private IEnumerator LoadCharacterInstance(InworldCharacter character, bool autoRecord = true)
        {
            void HandleStatusChange(InworldConnectionStatus status)
            {
                Debug.Log(status);
            }

            if (character == InworldController.CurrentCharacter) {
                yield break;
            }

            if (InworldController.CurrentCharacter != null) {
                InworldController.CurrentCharacter.CancelResponse();
                InworldController.Instance.StopAudio();
            }

            character.RegisterLiveSession();
            InworldController.CurrentCharacter = character;

            InworldController.Client.OnStatusChanged += HandleStatusChange;
            yield return new WaitForSeconds(0.5f);
            if (autoRecord) {
                InworldController.Instance.StartAudio();
            }
        }

        private IEnumerator InitializeControllerInstance()
        {
            InworldController.Instance.Reconnect();
            while (InworldController.Status != InworldConnectionStatus.Initialized) {
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }

        private IEnumerator LoadSceneInstance()
        {
            InworldController.Instance.LoadScene();
            while (InworldController.Status != InworldConnectionStatus.Connected) {
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }

        public static void SendTrigger(string triggerName, params string[] args)
        {
            if (InworldController.CurrentCharacter == null) {
                Debug.LogWarning("No character currently active");
                return;
            }
            SendTriggerToCharacter(triggerName, InworldController.CurrentCharacter.ID, args);
        }

        public static void SendTriggerToCharacter(string triggerName, string characterId, params string[] args)
        {
            if (args.Length == 0) {
                InworldController.Instance.SendTrigger(triggerName, characterId);
                return;
            }
            
            if (args.Length % 2 != 0) throw new FormatException("Args must be supplied in pairs");
            var output = new Dictionary<string, string>();
            for (var i = 0; i < args.Length; i += 2) {
                output.Add(args[i], args[i + 1]);
            }
            InworldController.Instance.SendTrigger(triggerName, characterId, output);
        }

        public static void SendText(string text)
        {
            if (InworldController.CurrentCharacter == null) {
                Debug.LogWarning("No character currently active");
                return;
            }
            SendTextToCharacter(text, InworldController.CurrentCharacter.ID);
        }

        public static void SendTextToCharacter(string text, string characterId)
        {
            InworldController.Instance.SendText(characterId, text);
        }
    }
}