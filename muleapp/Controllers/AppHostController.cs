using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mule.Controllers
{
    [Route("")]
    [Route("apphosts")]
    public class AppHostController : Controller
    {
        readonly IRepository<AppHost> AppHosts;
        public AppHostController(IRepository<AppHost> apphosts)
        {
            AppHosts = apphosts;
        }

        /// <summary>
        /// Called before each request
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            ViewData["Title"] = typeof(AppHost).Name;
            ViewData["Ponies"] = new Random().NextDouble() > 0.9;
            ViewData["Stopwatch"] = sw;
            base.OnActionExecuting(context);
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View(AppHosts.Read());
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create([Bind] AppHost item)
        {
            var existing = (from x in AppHosts.Read()
                            where x.Equals(item)
                            select x).FirstOrDefault();
            if (existing != null)
            {
                existing.Version = item.Version;
                existing.Owner = item.Owner;
                AppHosts.Update(existing);
            }
            else
            {
                AppHosts.Create(item);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("delete")]
        public IActionResult Delete([Bind] AppHost item)
        {
            var existing = from x in AppHosts.Read()
                            where x.Equals(item)
                            select x;
            foreach(var i in existing)
            {
                AppHosts.Delete(i);
            }
            return RedirectToAction("Index");
        }
    }
}
