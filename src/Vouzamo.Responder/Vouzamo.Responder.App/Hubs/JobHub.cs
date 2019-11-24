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
        protected JobPool Pool { get; }

        public JobHub(JobPool pool)
        {
            Pool = pool;
        }

        public async Task CompleteJob(Guid id, Response response)
        {
            if(Pool.TryCompleteJob(id, response))
            {
                await Clients.All.SendAsync("JobCompleted", id);
            }
        }
    }
}
