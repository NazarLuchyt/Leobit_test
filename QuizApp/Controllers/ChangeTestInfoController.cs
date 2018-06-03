using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using ModelClasses.Entities.Testing;
using ModelClasses.Entities.TestParts;
using QuizApp.ViewModel;
using QuizApp.ViewModel.Managing;
using QuizApp.ViewModel.Mapping;
using Services;

namespace QuizApp.Controllers
{
    [Authorize]
    public class ChangeTestInfoController : Controller
    {

        private readonly IGetInfoService _getInfoService;
        private readonly ILowLevelTestManagementService _lowLevelTestManagementService;
        private readonly IHighLevelTestManagementService _highLevelTestManagementService;

        private readonly IMapper _mapper;
        private readonly IAdvancedMapper _advancedMapper;

        public ChangeTestInfoController(IGetInfoService getInfoService,
            ILowLevelTestManagementService lowLevelTestManagementService,
            IHighLevelTestManagementService highLevelTestManagementService, IMapper mapper,
            IAdvancedMapper advancedMapper)
        {
            _getInfoService = getInfoService;
            _lowLevelTestManagementService = lowLevelTestManagementService;
            _highLevelTestManagementService = highLevelTestManagementService;
            _mapper = mapper;
            _advancedMapper = advancedMapper;
        }
        [HttpGet]
        public ActionResult GetAnswersByQuestionGuid(string questionGuid, string questionInstance)
        {
            var answerViewModelList = _getInfoService
                .GetQuestionByGuid(questionGuid)
                ?.TestAnswers
                .Select(a => _mapper.Map<AnswerViewModel>(a))
                .ToList();
            ViewBag.questionInstance = questionInstance;
            ViewBag.questionGuid = questionGuid;

            return View(answerViewModelList);
        }
        [HttpGet]
        public ActionResult CreateAnswer(string questionGuid, string questionInstance)
        {
            ViewBag.questionGuid = questionGuid;
            ViewBag.questionInstance = questionInstance;
            return View();
        }

        [HttpPost]
        public ActionResult CreateAnswer(string questionGuid, string questionInstance, AnswerViewModel answer)
        {
            var testAnswer = _mapper.Map<TestAnswer>(answer);
            _lowLevelTestManagementService.CreateAnswerForQuestion(questionGuid, testAnswer);
            return RedirectToAction("GetAnswersByQuestionGuid", "ChangeTestInfo", new { questionGuid = questionGuid, questionInstance = questionInstance });
        }
        [HttpPost]
        public ActionResult RemoveAnswer(string questionGuid, string answerGuid, string questionInstance)
        {
            _lowLevelTestManagementService.RemoveAnswer(answerGuid);
            return RedirectToAction("GetAnswersByQuestionGuid", "ChangeTestInfo", new { questionGuid = questionGuid, questionInstance= questionInstance });
        }

        [HttpGet]
        public ActionResult GetQuestionsByTestGuid(string testGuid)
        {
            var questionViewModelList = _getInfoService
                .GetTestByGuid(testGuid)
                ?.TestQuestions
                .Select(q => _advancedMapper.MapTestQuestion(q))
                .ToList();
            TransferViewModel<QuestionViewModel> transfer = new TransferViewModel<QuestionViewModel>(questionViewModelList);
           // transfer.TransferModel;
            ViewBag.testGuid = testGuid;
          //  foreach (QuestionViewModel a in transfer.TransferModel)
            //    ;
            return View(transfer);
        }
        [HttpGet]
        public ActionResult CreateQuestion(string testGuid)
        {
            ViewBag.testGuid = testGuid;
            return View();
        }

        [HttpPost]
        public ActionResult CreateQuestion(string testGuid, QuestionViewModel question)
        {
            var testQuestion = _mapper.Map<TestQuestion>(question);
            _lowLevelTestManagementService.CreateQuestionForTest(testGuid, testQuestion);
            return RedirectToAction("GetQuestionsByTestGuid", "ChangeTestInfo", new { testGuid = testGuid });
        }
        [HttpPost]
        public ActionResult RemoveQuestion(string testGuid, string questionGuid)
        {
            _lowLevelTestManagementService.RemoveQuestion(questionGuid);
            return RedirectToAction("GetQuestionsByTestGuid", "ChangeTestInfo", new { testGuid = testGuid });
        }
        [HttpPost]
        public ActionResult UpdateQuestion(string questionGuid, string testGuid, QuestionViewModel question)
        {
            var testQuestion = _mapper.Map<TestQuestion>(question);
            _lowLevelTestManagementService.UpdateQuestion(questionGuid, testQuestion);
            return RedirectToAction("GetQuestionsByTestGuid","ChangeTestInfo", new { testGuid = testGuid });
        }
        [HttpGet]
        public ActionResult GetTestByGuid(string testGuid)
        {
            TestViewModel temptest = _advancedMapper.MapTest(_getInfoService.GetTestByGuid(testGuid));
            if (temptest != null)
                return View(temptest);
            else
                return HttpNotFound();
        }
        [HttpGet]
        public ActionResult CreateTest()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult CreateTest(TestViewModel test)
        {
            var testFromDomain = _advancedMapper.MapTestViewModel(test);
            _highLevelTestManagementService.CreateTest(testFromDomain);
            return RedirectToAction("TestManagement", "Admin");
        }
        [HttpPost]
        public ActionResult UpdateTest(string testGuid, TestViewModel test)
        {
            var testFromDomain = _advancedMapper.MapTestViewModel(test);
            _highLevelTestManagementService.UpdateTest(testGuid, testFromDomain);
            return RedirectToAction("TestManagement", "Admin");
        }
        [HttpPost]
        public void RemoveTest(string testGuid)
        {
            _highLevelTestManagementService.RemoveTest(testGuid);
        }
        [HttpGet]
        public ActionResult CreateTestingUrl(string testGuid)
        {
            ViewBag.testGuid = testGuid;
            return View();
        }

        [HttpPost]
        public ActionResult CreateTestingUrl(TestingUrlViewModel testingUrl)
        {
            var testUrlDomain = _advancedMapper.MapTestingUrlViewModel(testingUrl);
            _highLevelTestManagementService.CreateTestingUrl(testUrlDomain);
            return RedirectToAction("TestingUrlManagement", "Admin");
        }
        [HttpPost]
        public void RemoveTestingUrl(string testingUrlGuid)
        {
            _highLevelTestManagementService.RemoveTestingUrl(testingUrlGuid);
        }


        [HttpPost]
        public void RemoveTestingResult(string testingResultGuid)
        {
            _highLevelTestManagementService.RemoveTestingResult(testingResultGuid);
        }

    }
}