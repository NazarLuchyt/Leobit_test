using QuizApp.Annotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.ViewModel.Managing
{
    public class TestViewModel
    {
        [Required(ErrorMessage = "Field Name must be fill")]
        public string Name { set; get; }
        [Required(ErrorMessage = "Field Description must be fill")]
        public string Description { set; get; }
        [Display(Name="Test time limit")]
        [MyTime(ErrorMessage = "Please enter corect time format for Test time limit")]
        public string TestTimeLimit { set; get; }
        [Display(Name = "Question time limit")]
        [MyTime(ErrorMessage = "Please enter corect time format  for Question time limit")]
        public string QuestionTimeLimit { set; get; }

        public List<QuestionViewModel> Questions { set; get; } 

        public string Guid { get; set; }

        public bool IsValid { get; set; }
    }
}
