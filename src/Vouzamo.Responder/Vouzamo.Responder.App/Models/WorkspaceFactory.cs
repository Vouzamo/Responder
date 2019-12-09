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
        protected OpenApiDocumentManager Manager { get; }

        public WorkspaceFactory(IMemoryCache cache, OpenApiDocumentManager manager)
        {
            Cache = cache;
            Manager = manager;
        }

        public async Task<Workspace> GetWorkspace(string key)
        {
            if(!Cache.TryGetValue(key, out Workspace workspace))
            {
                var ruleEngine = new RulesEngine();

                var rule = new OpenApiSpecificationRule(Manager)
                {
                    Name = "Test Rule",
                    SpecificationUri = new Uri("https://petstore.swagger.io/v2/swagger.json")
                    //SpecificationUri = new Uri("https://api.swaggerhub.com/apis/DEPTUSA/fwdusa-content-api/1.0.0/swagger.json")
                };

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
