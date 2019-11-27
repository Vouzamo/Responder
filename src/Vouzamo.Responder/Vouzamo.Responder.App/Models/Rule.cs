namespace Vouzamo.Responder.App.Models
{
    public abstract class Rule
    {
        public string Name { get; set; }

        public abstract bool IsMatch(Request request);
        public abstract Response GenerateResponse(Request request);
    }
}
