using System.ComponentModel.DataAnnotations;

namespace QuizApp.ViewModel.Managing
{
    public class AnswerViewModel
    {
        [Required(ErrorMessage = "Field Instance must be fill")]
        public string Instance { get; set; }
        public bool IsCorrect { get; set; }
        public string Guid { get; set; }
    }
}
