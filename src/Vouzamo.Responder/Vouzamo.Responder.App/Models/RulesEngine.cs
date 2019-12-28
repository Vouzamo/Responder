using FastDeepCloner;
using System.Collections.Generic;
using System.Linq;
using Vouzamo.Responder.App.Extensions;
using Vouzamo.Responder.App.Models.Rules;

namespace Vouzamo.Responder.App.Models
{
    public class RulesEngine
    {
        protected List<Rule> Rules { get; set; }

        public RulesEngine()
        {
            Rules = new List<Rule>();
        }

        public void RegisterRule(Rule rule)
        {
            Rules.Add(rule);
        }

        public bool TryMatchRule(Request request, out Rule match)
        {
            match = Rules.FirstOrDefault(rule => rule.IsMatch(request));

            if(match is default(Rule))
            {
                match = new DefaultRule();
            }

            return true;
        }

        public bool TryMatchRules(Request request, out IEnumerable<Rule> matches)
        {
            matches = Rules.Where(rule => rule.IsMatch(request));

            return matches.Any();
        }
    }
}