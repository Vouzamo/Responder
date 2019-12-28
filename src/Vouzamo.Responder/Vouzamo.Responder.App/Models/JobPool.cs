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

        public bool TryGetJob(Guid id, out Job job)
        {
            return Jobs.TryGetValue(id, out job);
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

        public void Clear()
        {
            Jobs.Clear();
        }
    }
}
