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
using QuizApp.ViewModel.HelpModels;

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
        public ActionResult GetAnswersByQuestionGuid(string questionGuid)
        {
            if (questionGuid != null)
            {
                var answerViewModelList = _getInfoService
                    .GetQuestionByGuid(questionGuid)
                    ?.TestAnswers
                    .Select(a => _mapper.Map<AnswerViewModel>(a))
                    .ToList();
                var models = new ListModelAndInfo<AnswerViewModel>();
                models.TransferModel = answerViewModelList;
                models.Guid = questionGuid;
               // models.HelpGuid = testGuid;
                return View(models);
            }
            return HttpNotFound();
        }
        [HttpGet]
        public ActionResult CreateAnswer(string questionGuid)
        {
            if (questionGuid != null)
            {
                var model = new ModelAndInfo<AnswerViewModel>();
                model.Guid = questionGuid;
               // model.HelpGuid = testGuid;
                return View(model);
            }
            return HttpNotFound();
        }   

        [HttpPost]
        public ActionResult CreateAnswer(string questionGuid, ModelAndInfo<AnswerViewModel> model)
        {
            if (ModelState.IsValid)
            {
                var testAnswer = _mapper.Map<TestAnswer>(model.TransferModel);
                _lowLevelTestManagementService.CreateAnswerForQuestion(questionGuid, testAnswer);
                if (questionGuid != null)
                {
                    return RedirectToAction(actionName: "GetAnswersByQuestionGuid",
                        controllerName: "ChangeTestInfo",
                        routeValues: new
                        {
                            questionGuid = questionGuid
                        });
                }
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult RemoveAnswer(string questionGuid, string answerGuid)
        {
            if (answerGuid != null)
            {
                _lowLevelTestManagementService.RemoveAnswer(answerGuid);
                return RedirectToAction(actionName: "GetAnswersByQuestionGuid",
                    controllerName: "ChangeTestInfo",
                    routeValues: new
                    {
                        questionGuid = questionGuid
                        //testGuid = testGuid
                    });
            }

            return HttpNotFound();
        }

        [HttpGet]
        public ActionResult GetQuestionsByTestGuid(string testGuid)
        {
            if (testGuid != null)
            {
                var questionViewModelList = _getInfoService
                    .GetTestByGuid(testGuid)
                    ?.TestQuestions
                    .Select(q => _advancedMapper.MapTestQuestion(q))
                    .ToList();
               var models = new ListModelAndInfo<QuestionViewModel>();
                models.TransferModel = questionViewModelList;
                models.Guid = testGuid;
                return View(models);
            }
            else
            {
                return HttpNotFound();
            }
         
        }
        [HttpGet]
        public ActionResult CreateQuestion(string testGuid)
        {
          if (testGuid != null)
            {
                var model = new ModelAndInfo<QuestionViewModel>
                {
                    TransferModel = new QuestionViewModel(),
                    Guid = testGuid
                };
                return View(model);
            }
                return HttpNotFound();
            
        }

        [HttpPost]
        public ActionResult CreateQuestion(string testGuid, ModelAndInfo<QuestionViewModel> model)
        {
            if (ModelState.IsValid)
            {  var testQuestion = _mapper.Map<TestQuestion>(model.TransferModel);
                _lowLevelTestManagementService.CreateQuestionForTest(testGuid, testQuestion);
                if (testGuid != null)
                {
                    return RedirectToAction(actionName: "GetQuestionsByTestGuid",
                        controllerName: "ChangeTestInfo",
                        routeValues: new
                        {
                            testGuid = testGuid
                        });
                }
                return HttpNotFound();
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult RemoveQuestion(string questionGuid)
        {
            if (questionGuid != null)
            {
                _lowLevelTestManagementService.RemoveQuestion(questionGuid);
                return RedirectToAction(actionName: "GetQuestionsByTestGuid",
                controllerName: "ChangeTestInfo",
                routeValues: new
                {
                    testGuid = Session["testGuid"]
                });
            }
            

            return HttpNotFound();
        }
        [HttpGet]
        public ActionResult UpdateQuestion(string questionGuid)
        {
            if( questionGuid != null)
            {
                var testQuestion = _advancedMapper.MapTestQuestion(_getInfoService.GetQuestionByGuid(questionGuid));

                var model = new ModelAndInfo<QuestionViewModel>();
                if (testQuestion != null)
                {
                    model.TransferModel = testQuestion;
                    model.Guid = questionGuid;
                    //model.HelpGuid = testGuid;
                    return View(model);
                }
            }
            return HttpNotFound();
        }
        [HttpPost]
        public ActionResult UpdateQuestion(ModelAndInfo<QuestionViewModel> model)
        {
            if (ModelState.IsValid)
            {
                var testQuestion = _mapper.Map<TestQuestion>(model.TransferModel);
                if (testQuestion != null)
                {
                    _lowLevelTestManagementService.UpdateQuestion(model.Guid, testQuestion);
                    return RedirectToAction(actionName: "GetQuestionsByTestGuid",
                        controllerName: "ChangeTestInfo",
                        routeValues: new
                        {
                           testGuid = Session["testGuid"]
                        });
                }
                return HttpNotFound();
                
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult GetTestByGuid(string testGuid)
        {
            TestViewModel temptest = _advancedMapper.MapTest(_getInfoService.GetTestByGuid(testGuid));
            if (temptest != null)
            {
                return View(temptest);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpGet]
        public ActionResult CreateTest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateTest(TestViewModel test)
        {
            if (ModelState.IsValid)
            {
                if (test.TestTimeLimit == null)
                {
                  test.TestTimeLimit = new TimeSpan().ToString();
                  test.QuestionTimeLimit = new TimeSpan().ToString();
                }
                var testFromDomain = _advancedMapper.MapTestViewModel(test);
                _highLevelTestManagementService.CreateTest(testFromDomain);
                if (testFromDomain != null)
                {
                    return RedirectToAction("TestManagement", "Admin");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                return View(test);
            }
        }
        [HttpGet]
        public ActionResult UpdateTest(string testGuid)
        {
            Session["testGuid"] = testGuid;
            TestViewModel test = _advancedMapper.MapTest(_getInfoService.GetTestByGuid(testGuid));
            if (test != null)
            {
                return View(test);
            }
            else
            {
                return HttpNotFound();
            }
        }


        [HttpPost]
        public ActionResult UpdateTest(string testGuid, TestViewModel test)
        {
            if (ModelState.IsValid)
            {
                var testFromDomain = _advancedMapper.MapTestViewModel(test);
                if (testFromDomain != null)
                {
                    _highLevelTestManagementService.UpdateTest(testGuid, testFromDomain);
                    return RedirectToAction("TestManagement", "Admin");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                return View(test);
            }
        }
        [HttpGet]
        public ActionResult RemoveTest(string testGuid)
        {
            if (testGuid != null)
            {
                _highLevelTestManagementService.RemoveTest(testGuid);
                return RedirectToAction(actionName: "TestManagement",
                    controllerName: "Admin");
            }
            return HttpNotFound();
        }
        [HttpGet]
        public ActionResult CreateTestingUrl(string testGuid)
        {
            var model = new ModelAndInfo<TestingUrlViewModel>();
            model.Guid = testGuid;
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateTestingUrl(ModelAndInfo<TestingUrlViewModel> model)
        {
            if (ModelState.IsValid)
            {
                var testUrlDomain = _advancedMapper.MapTestingUrlViewModel(model.TransferModel);
                _highLevelTestManagementService.CreateTestingUrl(testUrlDomain);
                return RedirectToAction("TestingUrlManagement", "Admin");
            }

            return View(model);
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