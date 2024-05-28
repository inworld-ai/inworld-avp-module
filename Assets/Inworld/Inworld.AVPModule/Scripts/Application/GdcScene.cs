using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Inworld;
using Inworld.Entities;
using LC.InworldUtils;
using UnityEngine;

namespace LC
{
    /// Manages the initialization of an Inworld scene
    public class GdcScene : MonoBehaviour
    {
        public string PlayerName = "Friend";
        public bool IsLoading;
        public InworldCharacter Character;
        public GameObject TranscriptCanvas;
        public string WelcomeTrigger = "welcome";

        public virtual void Start()
        {
            StartCoroutine(RunLoadRoutine());
        }

        protected virtual IEnumerator RunLoadRoutine()
        {
            IsLoading = true;

            //Wait for a frame to give the InworldController a moment to initialize
            yield return null;

            //Stop the current character speaking, and stop recording player audio
            if (InworldController.CurrentCharacter != null) InworldController.CurrentCharacter.CancelResponse();
            InworldController.Instance.StopAudio();

            //Setup the controller and load the scene
            InworldAI.User.Name = PlayerName;
            yield return InworldAsyncUtils.InitializeController();
            yield return InworldAsyncUtils.LoadScene();

            Character.transform.parent.gameObject.SetActive(true);
            yield return null; //one extra frame just to make extra sure it's initialized ;)
            yield return InworldAsyncUtils.LoadCharacter(Character, false);

            //Enable the audiocapture component now, which will open the microphone
            //We wait until this moment to work around some Unity bugs...maybe
            InworldController.Audio.enabled = true;

            TranscriptCanvas.SetActive(true);
            IsLoading = false;
            LoadingComplete();

            if (!string.IsNullOrEmpty(WelcomeTrigger)) {
                SendWelcomeMessage();
            }
        }

        protected virtual void LoadingComplete()
        {
            
        }

        protected virtual void SendWelcomeMessage()
        {
            if (string.IsNullOrEmpty(WelcomeTrigger)) return;
            InworldAsyncUtils.SendTrigger(WelcomeTrigger);
        }
    }
}