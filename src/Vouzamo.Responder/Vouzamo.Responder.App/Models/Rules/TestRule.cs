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

        public override Response GenerateResponse(Request request)
        {
            return new Response()
            {
                StatusCode = 200,
                ContentType = "application/json",
                Body = "{ \"success\": true, \"errors\": [] }"
            };
        }
    }
}
