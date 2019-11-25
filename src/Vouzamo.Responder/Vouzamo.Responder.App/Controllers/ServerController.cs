using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Readers;
using Vouzamo.Responder.App.Hubs;
using Vouzamo.Responder.App.Models;
using Vouzamo.Responder.App.Results;

namespace Vouzamo.Responder.App.Controllers
{
    [Route("server")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        protected IHubContext<JobHub> Hub { get; }
        protected JobPool Pool { get; }

        public ServerController(IHubContext<JobHub> hub, JobPool pool)
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

            if(Pool.TrySubmitJob(id, job))
            {
                await Hub.Clients.All.SendAsync("JobSubmitted", id, JsonSerializer.Serialize(job));
            }

            // wait for job completion
            while(!job.Handled)
            {
                Thread.Sleep(1000);
            }

            return new JobResponseActionResult(job.Response);
        }

        [Route("complete-job/{id}/{statusCode}/{message}")]
        public async Task<ActionResult> CompleteJob(Guid id, int statusCode, string message)
        {
            var response = new Response
            {
                StatusCode = statusCode,
                Body = message
            };

            if (Pool.TryCompleteJob(id, response))
            {
                await Hub.Clients.All.SendAsync("JobCompleted", id);

                return Accepted();
            }

            return NotFound();
        }


        [Route("create-api")]
        public async Task<ActionResult> CreateApi(string uri)
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(uri)
            };

            var response = await httpClient.GetAsync(string.Empty);
            var json = await response.Content.ReadAsStringAsync();

            var reader = new OpenApiStringReader();

            var document = reader.Read(json, out var diagnostic);

            return Ok(document.Info.Title);
        }
    }
}