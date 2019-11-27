using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vouzamo.Responder.App.Extensions;

namespace Vouzamo.Responder.App.Models.Rules
{
    public class OpenApiSpecificationRule : Rule
    {
        protected OpenApiDocument Specification { get; set; }

        public OpenApiSpecificationRule()
        {
            
        }

        public async Task LoadSpecification(string uri)
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(uri)
            };

            var response = await httpClient.GetAsync(string.Empty);
            var json = await response.Content.ReadAsStringAsync();

            var reader = new OpenApiStringReader();

            Specification = reader.Read(json, out var diagnostic);
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

        public override Response GenerateResponse(Request request)
        {
            foreach (var matchedPath in Specification.Paths.MatchPath(request))
            {
                if (matchedPath.PathItem.TryMatchOperation(request, out var operation))
                {
                    var operationResponse = operation.Responses.First();

                    if(int.TryParse(operationResponse.Key, out int statusCode))
                    {
                        var mediaType = operationResponse.Value.Content.First();

                        var response = new Response()
                        {
                            StatusCode = statusCode,
                            ContentType = mediaType.Key,
                            Body = mediaType.Value.Example?.ToString()
                        };

                        return response;
                    }
                }
            }

            throw new Exception("Couldn't generate response for OpenAPI rule.");
        }
    }
}
