using System;
using System.Collections;
using Inworld.Packet;
using LC.InworldUtils;
using Lunity;
using UnityEngine;

namespace LC.GameShow {

    public enum QuizCategory {
        None = -1,
        Art = 0,
        Entertainment = 1,
        Geography = 2,
        History = 3,
        Nature = 4,
        Sport = 5,
    }

    public enum QuizPhase
    {
        NotStarted = 0,
        Spinning = 1,
        Questioning = 2,
        Chatting = 4,
        ReadyToSpin = 5,
    }

    public class QuizGame : MonoBehaviour
    {
        [Header("References")]
        public CategorySpinner Spinner;

        [Header("Status")]
        public QuizPhase Phase = QuizPhase.NotStarted;
        [ReadOnly] public QuizCategory CurrentCategory = QuizCategory.None;
        [ReadOnly] public int CurrentQuestion = 1;

        private GameShowScene _scene;
        private bool _questionWasAnswered;
        private int _completedInteractionsThisPhase;

        public void Start()
        {
            _scene = FindObjectOfType<GameShowScene>();

            InworldCharacterEvents.OnCustom += HandleTrigger;
            InworldCharacterEvents.OnControl += HandleControl;
            InworldCharacterEvents.OnText += HandleText;
            
            //When we start spinning, load the appropriate question set in the Inworld brain
            Spinner.OnSpinStart += HandleSpinBegin;
            Spinner.OnSpinEnd += _ => AskQuestion();
            
            Spinner.SetCanSpin(false);
        }

        //If we've yet to spin, or finished answering a question and Quinnundrum says 'button', enable it!
        private void HandleText(TextPacket packet)
        {
            if (packet.routing.source.name == "player") return;
            if (Spinner.CanSpin) return;
            if (Phase != QuizPhase.NotStarted && Phase != QuizPhase.Chatting) return;
            if (!packet.text.text.ToLower().Contains("button")) return;
            
            GoToNextQuestion();
        }

        private void HandleTrigger(CustomPacket packet)
        {
            Debug.Log(packet.custom.name);
            var isComplete = packet.custom.name.Contains(".goal.") && packet.custom.name.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries)[2] == "complete";
            switch (packet.TriggerName) {
                case "ask_question":
                    //When ask question completes, that means that the question has been fully asked and we can start listening for an answer
                    //If it was interrupted, then the player may have said something other than an answer, so we send a different followup
                    //that doesn't assume they answered a quiz question
                    Debug.Log("Ask question goal " + (isComplete ? "complete" : "interrupted"));
                    _questionWasAnswered = isComplete;
                    AskFollowupQuestion(_questionWasAnswered);
                    Phase = QuizPhase.Chatting;
                    _completedInteractionsThisPhase = 0;
                    break;
                case "followup_question_complete":
                case "followup_question_interrupted":
                    //This trigger occurs at the end of the follow-up question goal
                    //It means we can move on to the next question and re-enable the spinner
                    Debug.Log("Followup question goal " + (isComplete ? "complete" : "interrupted"));
                    if(Phase != QuizPhase.ReadyToSpin && Phase != QuizPhase.Spinning && Phase != QuizPhase.Questioning) GoToNextQuestion();
                    break;
                default:
                    //Debug.Log((isComplete ? "Finished" : "Interrupted") + " trigger " + packet.TriggerName + " (" + packet.custom.name + ")");
                    break;
            }
        }

        private void HandleControl(ControlPacket packet)
        {
            Debug.Log("Interaction end");
            _completedInteractionsThisPhase++;
            if (_completedInteractionsThisPhase > 4 && Phase != QuizPhase.ReadyToSpin && Phase != QuizPhase.Spinning) {
                //we've had three back-and-forths since the last spin, so we should just show the button again now!
                GoToNextQuestion();
            }
        }

        //This method is raised when the player begins spinning for a new category
        //We send the chosen category to Innequin so that it can load the corresponding knowledge set
        private void HandleSpinBegin(int category)
        {
            CurrentCategory = (QuizCategory) category;
            _scene.SetCategory(CurrentCategory);
            Phase = QuizPhase.Spinning;
            _completedInteractionsThisPhase = 0;
            Spinner.CanSpin = false;
            StartCoroutine(ForceSpinnerEnabled());
        }

        //Just in case, for any reason, we never progress any further through the conversation,
        //we force the button to re-enable after 90 seconds so that the scene doesn't softlock
        private IEnumerator ForceSpinnerEnabled()
        {
            var timeoutTime = Time.time + 60f;
            while (Time.time < timeoutTime) {
                if (Spinner.CanSpin) yield break;
                yield return null;
            }
            GoToNextQuestion();
        }
        
        //This method is triggered when the spinner stops spinning. It will tell Innequin to ask the question
        //Within the goal is a trigger that will be received by Unity to begin listening for the player's answer
        private void AskQuestion()
        {
            _scene.AskQuestion(CurrentCategory, CurrentQuestion);
            Phase = QuizPhase.Questioning;
            _completedInteractionsThisPhase = 0;
        }

        //We manually listen for the end of the action following the player's response to Innequin's question
        //in order to request a follow-up question.
        //We split these apart to have Innequin be more likely to say 'dingding' or 'wahwah' specifically
        private void AskFollowupQuestion(bool questionWasAnswered)
        {
            _scene.AskFollowupQuestion(questionWasAnswered);
            Phase = QuizPhase.Chatting;
            _completedInteractionsThisPhase = 0;
        }

        //At the end of the follow-up question goals, a trigger is raised which results in this method being called
        //It allows the spinner to be interacted with again and updates the Inworld scene with the player's current score
        private void GoToNextQuestion()
        {
            Debug.Log("Ready to spin again!");
            Phase = QuizPhase.ReadyToSpin;
            _completedInteractionsThisPhase = 0;
            Spinner.SetCanSpin(true);
            CurrentQuestion++;
        }
    }
}