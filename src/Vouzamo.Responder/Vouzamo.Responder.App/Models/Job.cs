using System.Collections.Generic;
using Vouzamo.Responder.App.Models.Rules;

namespace Vouzamo.Responder.App.Models
{
    public class Job
    {
        public string Workspace { get; protected set; }
        public bool Handled => Response != null;
        public Request Request { get; }
        public List<RuleInput> Inputs { get; set; }
        public Dictionary<string, object> UserInput { get; set; }
        public Response Response { get; set; }

        public Job()
        {

        }

        public Job(string workspace, Request request) : this()
        {
            Workspace = workspace;
            Request = request;
            Inputs = new List<RuleInput>();
            UserInput = new Dictionary<string, object>();
        }
    }
}
