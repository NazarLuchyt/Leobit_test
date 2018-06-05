using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using AutoMapper;
using QuizApp.ViewModel.Managing;
using QuizApp.ViewModel.Mapping;
using Services;

namespace QuizApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IGetInfoService _getInfoService;
        private readonly IAdvancedMapper _advancedMapper;
        private readonly IAdvancedLogicService _advancedLogicService;
        private readonly IMapper _mapper;

        public AdminController(IGetInfoService getInfoService, IAdvancedMapper advancedMapper,
            IAdvancedLogicService advancedLogicService, IMapper mapper)
        {
            _getInfoService = getInfoService;
            _advancedMapper = advancedMapper;
            _advancedLogicService = advancedLogicService;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            return View("ResultManagement");
        }

        public ActionResult TestManagement()
        {
            Session["testGuid"] = null;
           var allTests = _getInfoService.GetAllTests().Select(t => _advancedMapper.MapTest(t)).ToList();
            if (allTests != null)
            {
                return View(allTests);
            }
                return HttpNotFound();
        }

        public ActionResult TestingUrlManagement()
        {
            var testingsList = _getInfoService.GetAllTestingUrls();
            var parsedTestingsList = testingsList.Select(t => _advancedMapper.MapTestingUrl(t)).ToList();
            foreach(var test in parsedTestingsList)
            {
                test.UrlInstance = CreateUrlLink(test.Guid);
            }
            return View(parsedTestingsList);
        }
        public string CreateUrlLink(string testGuid)
        {
            var urlLink = Request.Url.Authority + "/Quiz/Quiz?guid=" + testGuid;

            return urlLink;
        }
        
        public ActionResult ResultManagement()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAllTestingResults()
        {
            var allResults =
                _getInfoService.GetAllTestingResults()
                    .Select(r => _mapper.Map<TestingResultViewModel>(r))
                    .ToList();
            return Json(allResults, JsonRequestBehavior.AllowGet);
        }

        public void GetResultsForTestCsv(string testGuid)
        {
            StringWriter oStringWriter = new StringWriter();
            oStringWriter.WriteLine("LoL line");
            Response.ContentType = "text/plain";

            Response.AddHeader("content-disposition", "attachment;filename=" +
                                                      $"test_results_for_{testGuid}.csv");
            Response.Clear();

            using (StreamWriter writer = new StreamWriter(Response.OutputStream, Encoding.UTF8))
            {
                _advancedLogicService.GetCsvResults(testGuid, writer);
            }
            Response.End();
        }
    }
}