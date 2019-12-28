using FastDeepCloner;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vouzamo.Responder.App.Converters;

namespace Vouzamo.Responder.App.Models.Rules
{
    public abstract class Rule
    {
        public string Name { get; set; }

        public abstract bool IsMatch(Request request);

        public abstract Task PrepareJob(Job job);
        public abstract Task<bool> TryProcessJob(Job job);
    }

    public abstract class UserInputRule : Rule
    {
        public List<RuleInput> Inputs { get; }

        public UserInputRule()
        {
            Inputs = new List<RuleInput>();
        }

        public override Task PrepareJob(Job job)
        {
            job.Inputs = DeepCloner.Clone(Inputs);

            return Task.CompletedTask;
        }
    }

    public class DefaultRule : UserInputRule
    {
        public DefaultRule()
        {
            Inputs.Add(new StringRuleInput("Status Code", true));
            Inputs.Add(new StringRuleInput("Content Type", true));
            Inputs.Add(new StringRuleInput("Body"));
        }

        public override async Task<bool> TryProcessJob(Job job)
        {
            if(job.UserInput.TryGetValue("Status Code", out object value))
            {
                if(int.TryParse(value as string, out int statusCode))
                {
                    var contentType = "text/plain";
                    var body = "";

                    if(job.UserInput.TryGetValue("Content Type", out object value2))
                    {
                        contentType = value2 as string;
                    }

                    if (job.UserInput.TryGetValue("Body", out object value3))
                    {
                        body = value3 as string;
                    }

                    job.Response = new Response()
                    {
                        StatusCode = statusCode,
                        ContentType = contentType,
                        Body = body
                    };

                    return true;
                }
            }

            return false;
        }

        public override bool IsMatch(Request request)
        {
            return true;
        }
    }

    [JsonConverter(typeof(RuleInputConverter))]
    public abstract class RuleInput
    {
        public abstract string Type { get; }
        public string Name { get; set; }
        public bool IsMandatory { get; set; }

        public RuleInput()
        {

        }

        public RuleInput(string name, bool isMandatory = false)
        {
            Name = name;
            IsMandatory = isMandatory;
        }
    }

    public class StringRuleInput : RuleInput
    {
        public override string Type => "string";

        public StringRuleInput() : base()
        {

        }

        public StringRuleInput(string name, bool isMandatory = false) : base(name, isMandatory)
        {

        }
    }

    public class SelectRuleInput : RuleInput
    {
        public override string Type => "select";

        public List<string> Options { get; set; }

        public SelectRuleInput() : base()
        {
            Options = new List<string>();
        }

        public SelectRuleInput(string name, bool isMandatory = false) : base(name, isMandatory)
        {
            Options = new List<string>();
        }
    }
}
