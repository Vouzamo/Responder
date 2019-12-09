using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Exceptions;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vouzamo.Responder.App.Models;

namespace Vouzamo.Responder.App.Extensions
{
    public static class OpenApiExtensions
    {
        public static IEnumerable<MatchedPath> MatchPath(this OpenApiPaths paths, Request request)
        {
            var urlPatternRegex = new Regex(@"({([^}]+)\})");

            foreach(var pathItem in paths)
            {
                var pathPatternRegex = new Regex($"^{urlPatternRegex.Replace(pathItem.Key, @"(?<$2>[^\/]+)")}$");

                var match = pathPatternRegex.Match(request.Path);

                if (match.Success)
                {
                    var urlTokens = pathPatternRegex
                        .GetGroupNames()
                        .Skip(1) // We don't want the default (catch all) group
                        .Select(key => new KeyValuePair<string, string>(key, match.Groups[key].Value))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    var matchedPath = new MatchedPath(pathItem.Value, urlTokens);

                    yield return matchedPath;
                }
            }
        }

        public static bool TryMatchOperation(this OpenApiPathItem pathItem, Request request, out OpenApiOperation operation)
        {
            operation = pathItem.Operations
                .Where(operation => operation.Key.Equals(request.ToOperationType()))
                .Select(operation => operation.Value)
                .SingleOrDefault();

            if(operation != default(OpenApiOperation))
            {
                return true;
            }

            return false;
        }

        public static OperationType ToOperationType(this Request request)
        {
            return (request.Method.ToLower()) switch
            {
                "get" => OperationType.Get,
                "put" => OperationType.Put,
                "post" => OperationType.Post,
                "delete" => OperationType.Delete,
                "options" => OperationType.Options,
                "head" => OperationType.Head,
                "patch" => OperationType.Patch,
                "trace" => OperationType.Trace,

                _ => OperationType.Get,
            };
        }

        public static async Task<OpenApiResolvedReference<T>> ResolveReference<T>(this OpenApiReference reference, OpenApiDocument source, OpenApiDocumentManager manager) where T : class, IOpenApiReferenceable
        {
            if (reference.IsExternal)
            {
                var document = await manager.GetDocument(new Uri(reference.ExternalResource));

                if (document != null)
                {
                    var referenceSegments = reference.Id.Split("/");

                    var externallyResolved = referenceSegments[1] switch
                    {
                        "schemas" => document.Components.Schemas[referenceSegments[2]] as T,

                        "responses" => document.Components.Responses[referenceSegments[2]] as T,

                        "parameters" => document.Components.Parameters[referenceSegments[2]] as T,

                        "examples" => document.Components.Examples[referenceSegments[2]] as T,

                        "requestbodies" => document.Components.RequestBodies[referenceSegments[2]] as T,

                        "headers" => document.Components.Headers[referenceSegments[2]] as T,

                        "securityschemes" => document.Components.SecuritySchemes[referenceSegments[2]] as T,

                        "links" => document.Components.Links[referenceSegments[2]] as T,

                        "callbacks" => document.Components.Callbacks[referenceSegments[2]] as T,

                        _ => throw new OpenApiException("Failed to resolve reference."),
                    };

                    return new OpenApiResolvedReference<T>(externallyResolved, document);
                }
            }

            var locallyResolved = source.ResolveReference(reference) as T;

            return new OpenApiResolvedReference<T>(locallyResolved, source);
        }

        public static object GetValue(this IOpenApiAny any)
        {
            return any.AnyType switch
            {
                AnyType.Primitive => (any as IOpenApiPrimitive).PrimitiveType switch
                {
                    PrimitiveType.Boolean => (any as OpenApiBoolean).Value,
                    PrimitiveType.String => (any as OpenApiString).Value,
                    PrimitiveType.DateTime => (any as OpenApiDateTime).Value,
                    PrimitiveType.Integer => (any as OpenApiInteger).Value,
                    _ => null,
                },
                AnyType.Array => (any as OpenApiArray).Select(element => element.GetValue()),
                AnyType.Object => (any as OpenApiObject).ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue()),
                _ => null,
            };
        }

        public static async Task<object> BuildExample(this OpenApiResolvedReference<OpenApiSchema> schema, OpenApiDocumentManager manager)
        {
            var example = new Dictionary<string, object>();

            foreach (var property in schema.Resolved.Properties)
            {
                if (property.Value.Example != null)
                {
                    var value = property.Value.Example.GetValue();

                    example.Add(property.Key, value);
                }
                else if (property.Value.Reference != null)
                {
                    var resolvedSchema = await property.Value.Reference.ResolveReference<OpenApiSchema>(schema.Document, manager);

                    example.Add(property.Key, await BuildExample(resolvedSchema, manager));
                }
            }

            return example;
        }
    }
}