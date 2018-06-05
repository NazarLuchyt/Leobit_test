using System;
using System.Collections.Generic;

namespace QuizApp.ViewModel.PassingQuiz
{
    public class CurrentTestPassingViewModel<T>
    {
        public string AttemptGuid { set; get; }
        public TimeSpan? TestTimeLimit { get; set; }
        public TimeSpan? QuestionTimeLimit { get; set; }
        public List<T> Questions { get; set; }
        public int QuestionsSize { get; set; }
      
    }
}
