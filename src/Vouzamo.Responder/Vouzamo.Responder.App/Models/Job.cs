namespace Vouzamo.Responder.App.Models
{
    public class Job
    {
        public string Workspace { get; protected set; }
        public bool Handled => Response != null;
        public Request Request { get; }
        public Response Response { get; set; }

        public Job(string workspace, Request request)
        {
            Workspace = workspace;
            Request = request;
        }

        public void Respond(Response response)
        {
            Response = response;
        }
    }
}
