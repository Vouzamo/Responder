namespace Vouzamo.Responder.App.Models.Rules
{
    public abstract class Rule
    {
        public string Name { get; set; }

        public abstract bool IsMatch(Request request);
        public abstract Response GenerateResponse(Request request);
    }
}
