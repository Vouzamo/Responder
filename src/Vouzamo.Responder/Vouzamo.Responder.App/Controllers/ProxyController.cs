using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Vouzamo.Responder.App.Hubs;
using Vouzamo.Responder.App.Models;
using Vouzamo.Responder.App.Results;

namespace Vouzamo.Responder.App.Controllers
{
    [Route("proxy")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        protected IHubContext<JobHub> Hub { get; }
        protected JobPool Pool { get; }

        public ProxyController(IHubContext<JobHub> hub, JobPool pool)
        {
            Hub = hub;
            Pool = pool;
        }

        [Route("{workspace}/{*url}")]
        public async Task<IActionResult> SubmitJob(string workspace)
        {
            var id = Guid.NewGuid();

            var request = new Request()
            {
                Method = Request.Method,
                Path = Request.Path,
                QueryString = Request.QueryString,
                Body = "",
                //Headers = Request.Headers.ToDictionary((kvp) => kvp.Key)
            };

            var job = new Job(workspace, request);

            if (Pool.TrySubmitJob(id, job))
            {
                await Hub.Clients.All.SendAsync("JobSubmitted", id, JsonSerializer.Serialize(job));
            }

            // wait for job completion
            while (!job.Handled)
            {
                Thread.Sleep(1000);
            }

            return new JobResponseActionResult(job.Response);
        }
    }
}