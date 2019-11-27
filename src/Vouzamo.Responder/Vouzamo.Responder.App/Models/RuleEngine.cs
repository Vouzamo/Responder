using System.Collections.Generic;
using System.Linq;

namespace Vouzamo.Responder.App.Models
{

    public class RuleEngine
    {
        protected List<Rule> Rules { get; set; }

        public RuleEngine()
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

            return match != default(Rule);
        }

        public bool TryMatchRules(Request request, out IEnumerable<Rule> matches)
        {
            matches = Rules.Where(rule => rule.IsMatch(request));

            return matches.Any();
        }
    }
}