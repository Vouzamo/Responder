using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Vouzamo.Responder.App.Extensions;

namespace Vouzamo.Responder.App.Models.Rules
{
    public class OpenApiSpecificationRule : Rule
    {
        protected OpenApiDocumentManager Manager { get; set; }
        protected OpenApiDocument Specification => Manager.GetDocument(SpecificationUri).Result;

        public Uri SpecificationUri { get; set; }

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

        public override async Task<Response> GenerateResponse(Request request)
        {
            foreach (var matchedPath in Specification.Paths.MatchPath(request))
            {
                if (matchedPath.PathItem.TryMatchOperation(request, out var operation))
                {
                    var operationResponse = operation.Responses
                        .OrderBy(response => Guid.NewGuid())
                        .First();

                    if(int.TryParse(operationResponse.Key, out int statusCode))
                    {
                        var mediaType = operationResponse.Value.Content.First();

                        var schema = await mediaType.Value.Schema.Reference.ResolveReference<OpenApiSchema>(Specification, Manager);

                        var example = await schema.BuildExample(Manager);

                        var response = new Response()
                        {
                            StatusCode = statusCode,
                            ContentType = mediaType.Key,
                            Body = JsonSerializer.Serialize(example)
                        };

                        return response;
                    }
                }
            }

            throw new Exception("Couldn't generate response for OpenAPI rule.");
        }
    }
}
