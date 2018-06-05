using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ModelClasses.Entities.TestParts;
using QuizApp.ViewModel.Managing;
using QuizApp.ViewModel.Mapping;
using Services;
using QuizApp.ViewModel.HelpModels;

namespace QuizApp.Controllers
{
    [Authorize]
    public class TestController : Controller
    {

        private readonly IGetInfoService _getInfoService;
        private readonly ILowLevelTestManagementService _lowLevelTestManagementService;
        private readonly IHighLevelTestManagementService _highLevelTestManagementService;

        private readonly IMapper _mapper;
        private readonly IAdvancedMapper _advancedMapper;

        public TestController(IGetInfoService getInfoService,
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
                var models = new ListModelAndInfo<AnswerViewModel>
                {
                    TransferModel = answerViewModelList,
                    Guid = questionGuid
                };
                return View(models);
            }
            return HttpNotFound();
        }

        [HttpGet]
        public ActionResult CreateAnswer(string questionGuid)
        {
            if (questionGuid != null)
            {
                var model = new ModelAndInfo<AnswerViewModel>
                {
                    Guid = questionGuid
                };
                return View(model);
            }
            return HttpNotFound();
        }   

        [HttpPost]
        public ActionResult CreateAnswer(string questionGuid, ModelAndInfo<AnswerViewModel> model)
        {
            if (!ModelState.IsValid) return View(model);
            if (questionGuid == null) return HttpNotFound();
            
            var testAnswer = _mapper.Map<TestAnswer>(model.TransferModel);
            _lowLevelTestManagementService.CreateAnswerForQuestion(questionGuid, testAnswer);
                
            return RedirectToAction(actionName: "GetAnswersByQuestionGuid",
                controllerName: "Test",
                routeValues: new
                {
                    QuestionGuid = questionGuid
                });
        }
        [HttpGet]
        public ActionResult RemoveAnswer(string questionGuid, string answerGuid)
        {
            if ( (answerGuid != null) && (questionGuid!=null) )
            {
                _lowLevelTestManagementService.RemoveAnswer(answerGuid);
                return RedirectToAction(actionName: "GetAnswersByQuestionGuid",
                    controllerName: "Test",
                    routeValues: new
                    {
                        QuestionGuid = questionGuid
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
                var models = new ListModelAndInfo<QuestionViewModel>
                {
                    TransferModel =  questionViewModelList,
                    Guid = testGuid

                };
                return View(models);
            }
            return HttpNotFound();
            
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
            {
                if (testGuid != null)
                {
                    var testQuestion = _mapper.Map<TestQuestion>(model.TransferModel);
                _lowLevelTestManagementService.CreateQuestionForTest(testGuid, testQuestion);
                
                    return RedirectToAction(actionName: "GetQuestionsByTestGuid",
                        controllerName: "Test",
                        routeValues: new
                        {
                            TestGuid = testGuid
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
                controllerName: "Test",
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
                if (testQuestion != null)
                {
                    var model = new ModelAndInfo<QuestionViewModel>
                    {
                        TransferModel = testQuestion,
                        Guid = questionGuid
                    };
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
                        controllerName: "Test",
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
        public ActionResult CreateTest()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateTest(TestViewModel test)
        {
            if (ModelState.IsValid)
            {
                if (test.TestTimeLimit == null) test.TestTimeLimit = new TimeSpan().ToString();
                if (test.QuestionTimeLimit == null) test.QuestionTimeLimit = new TimeSpan().ToString();
                var testFromDomain = _advancedMapper.MapTestViewModel(test);
                _highLevelTestManagementService.CreateTest(testFromDomain);
                if (testFromDomain != null)
                {
                    return RedirectToAction("TestManagement", "Admin");
                }

                return HttpNotFound();

            }
            return View(test);
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
           
            return HttpNotFound();
            
        }

        [HttpPost]
        public ActionResult UpdateTest(string testGuid, TestViewModel test)
        {
            if (ModelState.IsValid)
            {
                if (testGuid != null)
                {
                    if(test.TestTimeLimit == null) test.TestTimeLimit =  new TimeSpan().ToString();
                    if (test.QuestionTimeLimit == null) test.QuestionTimeLimit = new TimeSpan().ToString();
                    var testFromDomain = _advancedMapper.MapTestViewModel(test);
                    if (testFromDomain != null)
                    {
                        _highLevelTestManagementService.UpdateTest(testGuid, testFromDomain);
                        return RedirectToAction("TestManagement", "Admin");
                    }
                }

                return HttpNotFound();
                
            }
            return View(test);
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
            var model = new ModelAndInfo<TestingUrlViewModel>
            {
                Guid = testGuid
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateTestingUrl(ModelAndInfo<TestingUrlViewModel> model)
        {
            if (model != null)
            {
                if (model.TransferModel.AllowedEndDate == null) model.TransferModel.AllowedEndDate = new DateTime().ToString();
                if (model.TransferModel.AllowedStartDate == null) model.TransferModel.AllowedStartDate = new DateTime().ToString();
            }
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