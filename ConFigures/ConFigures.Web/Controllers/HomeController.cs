using System.Web.Mvc;

namespace ConFigures.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("Home/Index")]
        [Route("Index")]
        [Route("")]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
