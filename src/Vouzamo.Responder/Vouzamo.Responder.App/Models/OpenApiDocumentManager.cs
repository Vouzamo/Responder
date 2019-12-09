using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vouzamo.Responder.App.Models
{
    public class OpenApiDocumentManager
    {
        protected const string DefaultId = "default";

        public Action<HttpClient> PreRequest => (client) => client.DefaultRequestHeaders.Add("Authorization", "eaaebed3-297d-43fe-97aa-9f114cec160b");

        protected IMemoryCache Cache { get; }
        protected IHttpClientFactory HttpClientFactory { get; }
        

        public OpenApiDocumentManager(IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            Cache = cache;
            HttpClientFactory = httpClientFactory;
        }

        public void SetDocument(Uri uri, OpenApiDocument document)
        {
            Cache.Set(uri, document);
        }

        public async Task<OpenApiDocument> GetDocument(Uri uri)
        {
            if(!Cache.TryGetValue(uri, out OpenApiDocument document))
            {
                document = await LoadDocument(uri);

                if(document != null)
                {
                    SetDocument(uri, document);
                }
            }

            return document;
        }

        public async Task<OpenApiDocument> LoadDocument(Uri uri)
        {
            var client = HttpClientFactory.CreateClient();

            client.BaseAddress = uri;

            PreRequest.Invoke(client);

            var response = await client.GetAsync(string.Empty);
            var json = await response.Content.ReadAsStringAsync();

            var reader = new OpenApiStringReader();

            return reader.Read(json, out var diagnostic);
        }
    }
}
