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
        protected WorkspaceFactory WorkspaceFactory { get; }

        public ProxyController(IHubContext<JobHub> hub, WorkspaceFactory workspaceFactory)
        {
            Hub = hub;
            WorkspaceFactory = workspaceFactory;
        }

        [Route("{workspaceKey}/{*url}"), HttpGet, HttpPut, HttpPost, HttpDelete]
        public async Task<IActionResult> SubmitJob(string workspaceKey, string url)
        {
            var id = Guid.NewGuid();

            var request = new Request()
            {
                Method = Request.Method,
                Path = $"/{url}",
                QueryString = Request.QueryString,
                Body = "",
                //Headers = Request.Headers.ToDictionary((kvp) => kvp.Key)
            };

            var workspace = await WorkspaceFactory.GetWorkspace(workspaceKey);

            var job = new Job(workspaceKey, request);

            if (workspace.JobPool.TrySubmitJob(id, job))
            {
                await Hub.Clients.All.SendAsync("JobSubmitted", id, JsonSerializer.Serialize(job));
            }

            if (workspace.Options != WorkspaceOptions.InterceptNone)
            {
                if(workspace.RuleEngine.TryMatchRule(request, out var rule))
                {
                    if (workspace.JobPool.TryCompleteJob(id, rule.GenerateResponse(request)))
                    {
                        await Hub.Clients.All.SendAsync("JobCompleted", id);
                    }
                }
            }

            while (!job.Handled)
            {
                Thread.Sleep(1000);
            }

            return new JobResponseActionResult(job.Response);
        }
    }
}