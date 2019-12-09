using System.Threading.Tasks;

namespace Vouzamo.Responder.App.Models.Rules
{
    public abstract class Rule
    {
        public string Name { get; set; }

        public abstract bool IsMatch(Request request);
        public abstract Task<Response> GenerateResponse(Request request);
    }
}
