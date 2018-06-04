using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.ViewModel.Managing
{
    public class QuestionViewModel
    {
        [Required(ErrorMessage = "Field Instance must be fill")]
        public string Instance { get; set; }
        [Required(ErrorMessage = "Field Hint must be fill")]
        public string Hint { set; get; }
        public List<AnswerViewModel> Answers { get; set; }
        public bool IsValid { get; set; }
        public string Guid { get; set; }
    }
}
