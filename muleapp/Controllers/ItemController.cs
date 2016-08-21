using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Mule
{
    public abstract class ItemController<T> : Controller where T : class
    {
        readonly IRepository<T> Items;

        public ItemController(IRepository<T> repository)
        {
            Items = repository;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View("Items/Index",Items.Read());
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create([Bind] T item)
        {
            var existing = (from x in Items.Read()
                            where x.Equals(item)
                            select x).FirstOrDefault();
            if (existing != null)
            {
                Items.Update(existing, item);
            }
            else
            {
                Items.Create(item);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("delete")]
        public IActionResult Delete([Bind] T item)
        {
            var existing = from x in Items.Read()
                           where x.Equals(item)
                           select x;
            foreach (var i in existing)
            {
                Items.Delete(i);
            }
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Called before each request
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sw = new Stopwatch();
            sw.Start();
            ViewData["Title"] = typeof(Weather).Name;
            ViewData["Ponies"] = new Random().NextDouble() > 0.95;
            ViewData["Nyan"] = new Random().NextDouble() > 0.85;
            ViewData["Stopwatch"] = sw;
            base.OnActionExecuting(context);
        }
    }
}
