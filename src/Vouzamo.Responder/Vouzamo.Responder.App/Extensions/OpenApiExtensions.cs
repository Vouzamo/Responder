using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    }
}
