using System.Threading.Tasks;

namespace Vouzamo.Responder.App.Models.Rules
{
    public class TestRule : Rule
    {
        public TestRule()
        {
            
        }

        public override bool IsMatch(Request request)
        {
            return request.Path == "/match-this";
        }

        public override async Task<bool> TryProcessJob(Job job)
        {
            job.Response = new Response()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = "{ \"success\": true, \"errors\": [] }"
            };

            return true;
        }

        public override Task PrepareJob(Job job)
        {
            return Task.CompletedTask;
        }
    }
}
