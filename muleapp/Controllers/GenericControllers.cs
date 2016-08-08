using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Chroniton.Schedules;
using Chroniton.Jobs;
using System.Net.NetworkInformation;

namespace Mule.Controllers
{
    [Route("")]
    [Route("apphosts")]
    public class AppHostController : ItemController<AppHost>
    {
        public AppHostController(IRepository<AppHost> repository, Chroniton.ISingularity scheduler, IMemoryCache cache) : base(repository)
        {
            foreach (var item in repository.Read()) //Update transient model properties from background tasks, etc
            {
                var rtt = 0;
                if(!cache.TryGetValue(item.URL, out rtt)){ //Load the ping value from the cache if it exists
                    cache.Set(item.URL, -1); //Set an intial value if the Ping fails

                    scheduler.ScheduleJob(new ConstantSchedule(), new SimpleJob(
                        time => cache.Set(item.URL, (int)new Ping().Send(item.URL).RoundtripTime)
                    ),
                    runImmediately: true); //Start background job to monitor RTT to host URLs
                }
                item.RTT = rtt; //Set RTT from cache or background job
            }
        }
    }
}
