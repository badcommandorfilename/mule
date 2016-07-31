using Microsoft.AspNetCore.Mvc;

namespace Mule.Controllers
{
    [Route("home")]
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
