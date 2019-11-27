namespace Vouzamo.Responder.App.Models
{
    public class Workspace
    {
        public JobPool JobPool { get; set; }
        public RuleEngine RuleEngine { get; set; }
        public WorkspaceOptions Options { get; set; }

        public Workspace()
        {
            Options = WorkspaceOptions.InterceptUnmatched;
        }
    }
}
