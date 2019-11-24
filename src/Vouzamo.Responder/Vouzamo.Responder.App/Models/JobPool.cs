using System;
using System.Collections.Generic;

namespace Vouzamo.Responder.App.Models
{
    public class JobPool
    {
        protected Dictionary<Guid, Job> Jobs { get; }

        public JobPool()
        {
            Jobs = new Dictionary<Guid, Job>();
        }

        public bool TrySubmitJob(Guid id, Job job)
        {
            if(!Jobs.ContainsKey(id))
            {
                Jobs.Add(id, job);

                return true;
            }

            return false;
        }

        public bool TryCompleteJob(Guid id, Response response)
        {
            if (Jobs.TryGetValue(id, out var job))
            {
                job.Respond(response);

                return true;
            }

            return false;
        }

        public void Clear()
        {
            Jobs.Clear();
        }
    }
}
