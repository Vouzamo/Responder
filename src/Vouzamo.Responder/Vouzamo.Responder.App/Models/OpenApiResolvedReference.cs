using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace Vouzamo.Responder.App.Models
{
    public class OpenApiResolvedReference<T> where T : IOpenApiReferenceable
    {
        public T Resolved { get; protected set; }
        public OpenApiDocument Document { get; protected set; }

        public OpenApiResolvedReference(T resolved, OpenApiDocument document)
        {
            Resolved = resolved;
            Document = document;
        }
    }
}
