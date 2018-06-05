using System.ComponentModel.DataAnnotations;
using QuizApp.Annotations;

namespace QuizApp.ViewModel.Managing
{
    public class TestingUrlViewModel
    {
        public string UrlInstance { set; get; }
        public string Guid { set; get; }
        public string TestGuid { set; get; }
        public string TestName { set; get; }
        public string Interviewee { set; get; }
        [MyNumber(ErrorMessage = "Enter corect number")]
        public int NumberOfRuns { set; get; }
        [MyDateTime(ErrorMessage = "Please enter corect Date or Time")]
        public string AllowedStartDate { set; get; }
        [MyDateTime(ErrorMessage = "Please enter corect Date or Time")]
        public string AllowedEndDate { set; get; }
        public bool IsValid { get; set; }
    }
}