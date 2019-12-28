using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Readers;
using Vouzamo.Responder.App.Hubs;
using Vouzamo.Responder.App.Models;
using Vouzamo.Responder.App.Models.Rules;

namespace Vouzamo.Responder.App.Controllers
{

    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        protected IHubContext<JobHub> Hub { get; }
        protected WorkspaceFactory WorkspaceFactory { get; }

        public ApiController(IHubContext<JobHub> hub, WorkspaceFactory workspaceFactory)
        {
            Hub = hub;
            WorkspaceFactory = workspaceFactory;
        }

        [HttpPost("{workspaceKey}/complete-job/{id}")]
        public async Task<ActionResult> CompleteJob(string workspaceKey, Guid id, [FromBody] Dictionary<string, object> userInputs)
        {
            if(!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var workspace = await WorkspaceFactory.GetWorkspace(workspaceKey);

            if(workspace.JobPool.TryGetJob(id, out Job job))
            {
                job.UserInput = userInputs;

                if (workspace.RuleEngine.TryMatchRule(job.Request, out Rule rule))
                {
                    if (await rule.TryProcessJob(job))
                    {
                        return Accepted();
                    }
                }
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