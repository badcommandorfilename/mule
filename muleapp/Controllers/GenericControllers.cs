using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Mule.Controllers
{
    [Route("")]
    [Route("apphosts")]
    public class AppHostController : ItemController<AppHost>
    {
        //Declare controllers for routing
        public AppHostController(IRepository<AppHost> repository) : base(repository)
        {
        }
    }
}
