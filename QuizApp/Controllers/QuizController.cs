using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using QuizApp.ViewModel.HelpModels;
using QuizApp.ViewModel.Managing;
using QuizApp.ViewModel.Mapping;
using Services;
using QuizApp.ViewModel.PassingQuiz;

namespace QuizApp.Controllers
{
    public class QuizController : Controller
    {
        private readonly IGetInfoService _getInfoService;
        private readonly IAdvancedLogicService _advancedLogicService;
        private readonly IMapper _mapper;
        private readonly IAdvancedMapper _advancedMapper;
        private static TestPassingViewModel CurrentTest;

        public QuizController(IGetInfoService getInfoService, IAdvancedLogicService advancedLogicService,
            IMapper mapper, IAdvancedMapper advancedMapper)
        {
            _getInfoService = getInfoService;
            _advancedLogicService = advancedLogicService;
            _mapper = mapper;
            _advancedMapper = advancedMapper;
        }


        public ActionResult Quiz(string guid)
        {
            var testUrlDomain = _getInfoService.GetTestingUrlByGuid(guid);
            var error = _advancedLogicService.CheckTestingUrlForAvailability(testUrlDomain);
            if (!string.IsNullOrEmpty(error))
            {
                return View("TestingErrorView",(object)error);
            }
            //if all is ok
            var testUrl = _advancedMapper.MapTestingUrl(testUrlDomain);
            var model = new ModelAndInfo<TestingUrlViewModel>
            {
                TransferModel = testUrl,
                Guid = guid
            };
            CurrentTest = new TestPassingViewModel()
            {
                TestingStartDateTime = DateTime.Now.ToString(),
                Interviewee = testUrl.Interviewee,
                TestingGuid = testUrl.TestGuid
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult GetInfoAndStartTest(string testingUrlGuid)
        {
            if (testingUrlGuid != null)
            {
                var domainTest = _getInfoService.GetTestByTestingUrlGuid(testingUrlGuid);

                if (domainTest != null)
                {
                    var questionViewModelList = domainTest
                        ?.TestQuestions
                        .Select(q => _mapper.Map<QuestionPassingViewModel>(q))
                        .ToList();

                    var attepmtGuid = Guid.NewGuid().ToString();

                    var test = new CurrentTestPassingViewModel<QuestionPassingViewModel>()
                    {
                        TestTimeLimit = domainTest.TestTimeLimit ?? new TimeSpan(),
                        QuestionTimeLimit = domainTest.QuestionTimeLimit ?? new TimeSpan(),
                        Questions = questionViewModelList,
                        AttemptGuid = attepmtGuid,
                        QuestionsSize = questionViewModelList.Count
                    };
                    CurrentTest.AttemptGuid = attepmtGuid;

                    _advancedLogicService.StartQuiz(_getInfoService.GetTestingUrlByGuid(testingUrlGuid), attepmtGuid);

                    // return Json(test, JsonRequestBehavior.AllowGet);
                    return View(test.Questions);
                }
            }
            return HttpNotFound();
        }


        [HttpPost]
        public void FinishTest(TestPassingViewModel testPassing)
        {
            var testPassingMapped = _advancedMapper.MapTestPassingViewModel(testPassing);
            _advancedLogicService.FinishQuiz(testPassingMapped);
        }
    }
}