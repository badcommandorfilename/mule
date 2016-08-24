using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Mule.Controllers
{
    [Route("error")]
    public class ErrorsController : Controller
    {
        [Route("")]
        [Route("{errCode}")]
        public IActionResult Index(string errCode)
        {
            ViewData["Message"] = errCode ?? "Error";
            return View();
        }
    }
}
