using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Vouzamo.Responder.App.Models.Rules;

namespace Vouzamo.Responder.App.Models
{
    public class WorkspaceFactory
    {
        protected IMemoryCache Cache { get; }

        public WorkspaceFactory(IMemoryCache cache)
        {
            Cache = cache;
        }

        public async Task<Workspace> GetWorkspace(string key)
        {
            if(!Cache.TryGetValue(key, out Workspace workspace))
            {
                var ruleEngine = new RulesEngine();

                var rule = new OpenApiSpecificationRule()
                {
                    Name = "Test Rule",
                };

                await rule.LoadSpecification("https://petstore.swagger.io/v2/swagger.json");

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
