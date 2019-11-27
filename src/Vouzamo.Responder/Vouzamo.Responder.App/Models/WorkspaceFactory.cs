using Microsoft.Extensions.Caching.Memory;

namespace Vouzamo.Responder.App.Models
{
    public class WorkspaceFactory
    {
        protected IMemoryCache Cache { get; }

        public WorkspaceFactory(IMemoryCache cache)
        {
            Cache = cache;
        }

        public Workspace GetWorkspace(string key)
        {
            if(!Cache.TryGetValue(key, out Workspace workspace))
            {
                var ruleEngine = new RuleEngine();

                ruleEngine.RegisterRule(new TestRule()
                {
                    Name = "Test Rule",
                });

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
