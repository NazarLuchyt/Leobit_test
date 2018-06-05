using System.ComponentModel.DataAnnotations;
using QuizApp.Annotations;

namespace QuizApp.ViewModel.Managing
{
    public class TestingUrlViewModel
    {
        public string UrlInstance { set; get; }
        public string Guid { set; get; }
        public string TestGuid { set; get; }
        [Display(Name="Test name:")]
        public string TestName { set; get; }
        public string Interviewee { set; get; }
        [Display(Name = "Number of runs:")]
        [MyNumber(ErrorMessage = "Enter corect number")]
        public int NumberOfRuns { set; get; }
        [Display(Name = "Allowed start date:")]
        [MyDateTime(ErrorMessage = "Please enter corect Date or Time")]
        public string AllowedStartDate { set; get; }
        [Display(Name = "Allowed end date:")]
        [MyDateTime(ErrorMessage = "Please enter corect Date or Time")]
        public string AllowedEndDate { set; get; }
        public bool IsValid { get; set; }
    }
}