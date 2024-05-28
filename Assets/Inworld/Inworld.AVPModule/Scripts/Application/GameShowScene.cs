using System.Collections.Generic;
using Inworld;
using LC.InworldUtils;
using LC.UI;
using Lunity;

namespace LC.GameShow
{
    public class GameShowScene : GdcScene
    {
        public void SetCategory(QuizCategory category)
        {
            InworldAsyncUtils.SendTrigger("set_knowledge_" + category.ToString().ToLower());
        }
        
        public void AskQuestion(QuizCategory category, int questionNumber)
        {
            InworldAsyncUtils.SendTrigger("ask_question", "category", category.ToString(), "question_number", questionNumber.ToString());
        }

        public void AskFollowupQuestion(bool questionWasAnswered)
        {
            InworldAsyncUtils.SendTrigger("followup_question_" + (questionWasAnswered ? "complete" : "interrupted"));
        }
        
        [EditorButton]
        public void Spin()
        {
            GetComponentInChildren<DebuggableTouchReceiver>().DebugTouchDown();
        }
    }
}