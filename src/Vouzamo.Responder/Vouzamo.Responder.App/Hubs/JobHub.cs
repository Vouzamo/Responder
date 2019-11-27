using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Vouzamo.Responder.App.Models;

namespace Vouzamo.Responder.App.Hubs
{
    public class JobHub : Hub
    {
        protected WorkspaceFactory WorkspaceFactory { get; }

        public JobHub(WorkspaceFactory workspaceFactory)
        {
            WorkspaceFactory = workspaceFactory;
        }

        //public async Task CompleteJob(Guid id, Response response)
        //{
        //    if(Pool.TryCompleteJob(id, response))
        //    {
        //        await Clients.All.SendAsync("JobCompleted", id.ToString());
        //    }
        //}
    }
}
