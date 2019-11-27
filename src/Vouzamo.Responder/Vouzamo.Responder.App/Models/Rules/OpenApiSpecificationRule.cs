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
        public Uri SpecificationUri { get; set; }
        public Action<HttpClient> PreRequest { get; set; }

        protected IHttpClientFactory HttpClientFactory { get; }
        protected OpenApiDocument Specification { get; set; }

        public OpenApiSpecificationRule(IHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        public async Task LoadSpecification()
        {
            var client = HttpClientFactory.CreateClient();

            client.BaseAddress = SpecificationUri;
            
            PreRequest.Invoke(client);

            var response = await client.GetAsync(string.Empty);
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
                    var operationResponse = operation.Responses
                        .OrderBy(response => Guid.NewGuid())
                        .First();

                    if(int.TryParse(operationResponse.Key, out int statusCode))
                    {
                        var mediaType = operationResponse.Value.Content.First();

                        var response = new Response()
                        {
                            StatusCode = statusCode,
                            ContentType = mediaType.Key,
                            Body = mediaType.Value.Schema.Reference.Id
                        };

                        return response;
                    }
                }
            }

            throw new Exception("Couldn't generate response for OpenAPI rule.");
        }
    }
}
