using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mule.Controllers
{
    [Route("")]
    [Route("apphosts")]
    public class AppHostController : ItemController<AppHost>
    {
        public AppHostController(IRepository<AppHost> repository) : base(repository)
        {

        }
    }
}
