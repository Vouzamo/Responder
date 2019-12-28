using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Vouzamo.Responder.App.Extensions;

namespace Vouzamo.Responder.App.Models.Rules
{
    public class OpenApiSpecificationRule : UserInputRule
    {
        public OpenApiDocumentManager Manager { get; protected set; }
        public Uri SpecificationUri { get; set; }

        protected OpenApiDocument Specification => Manager.GetDocument(SpecificationUri).Result;

        public OpenApiSpecificationRule(OpenApiDocumentManager manager)
        {
            Manager = manager;
        }

        public override bool IsMatch(Request request)
        {
            foreach(var matchedPath in Specification.Paths.MatchPath(request))
            {
                if(matchedPath.PathItem.TryMatchOperation(request, out var operation))
                {
                    return !operation.Deprecated;
                }
            }

            return false;
        }

        public override async Task PrepareJob(Job job)
        {
            await base.PrepareJob(job);

            foreach (var matchedPath in Specification.Paths.MatchPath(job.Request))
            {
                if (matchedPath.PathItem.TryMatchOperation(job.Request, out var operation))
                {
                    var selectInput = new SelectRuleInput("Operation Response", true);
                    selectInput.Options.AddRange(operation.Responses.Keys);

                    job.Inputs.Add(selectInput);
                }
            }
        }

        public override async Task<bool> TryProcessJob(Job job)
        {
            foreach (var matchedPath in Specification.Paths.MatchPath(job.Request))
            {
                if (matchedPath.PathItem.TryMatchOperation(job.Request, out var operation))
                {
                    if(job.UserInput.TryGetValue("Operation Response", out object input))
                    {
                        var inputAsString = input as string;

                        if (operation.Responses.TryGetValue(inputAsString, out var operationResponse))
                        {
                            if (int.TryParse(inputAsString, out int statusCode))
                            {
                                var mediaType = operationResponse.Content.First();

                                var body = "";

                                if (mediaType.Value.Schema != null)
                                {
                                    var schema = await mediaType.Value.Schema.Reference.ResolveReference<OpenApiSchema>(Specification, Manager);

                                    var example = await schema.BuildExample(Manager);

                                    body = JsonSerializer.Serialize(example);
                                }

                                job.Response = new Response()
                                {
                                    StatusCode = statusCode,
                                    ContentType = mediaType.Key,
                                    Body = body
                                };

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
