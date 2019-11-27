using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Vouzamo.Responder.App.Models.Rules;

namespace Vouzamo.Responder.App.Models
{
    public class WorkspaceFactory
    {
        protected IMemoryCache Cache { get; }
        protected IHttpClientFactory HttpClientFactory { get; }

        public WorkspaceFactory(IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            Cache = cache;
            HttpClientFactory = httpClientFactory;
        }

        public async Task<Workspace> GetWorkspace(string key)
        {
            if(!Cache.TryGetValue(key, out Workspace workspace))
            {
                var ruleEngine = new RulesEngine();

                var rule = new OpenApiSpecificationRule(HttpClientFactory)
                {
                    Name = "Test Rule",
                    SpecificationUri = new Uri("https://api.swaggerhub.com/apis/DEPTUSA/fwdusa-content-api/1.0.0/swagger.json"),
                    PreRequest = (client) => client.DefaultRequestHeaders.Add("Authorization", "eaaebed3-297d-43fe-97aa-9f114cec160b")
                };

                await rule.LoadSpecification();

                ruleEngine.RegisterRule(rule);

                workspace = new Workspace()
                {
                    JobPool = new JobPool(),
                    RuleEngine = ruleEngine
                };

                Cache.Set(key, workspace);
            }

            return workspace;
        }
    }
}
