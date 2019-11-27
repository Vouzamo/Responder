using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace Vouzamo.Responder.App.Models
{
    public class MatchedPath
    {
        public OpenApiPathItem PathItem { get; set; }
        public Dictionary<string, string> UrlTokens { get; set; }

        public MatchedPath(OpenApiPathItem pathItem, Dictionary<string, string> urlTokens)
        {
            PathItem = pathItem;
            UrlTokens = urlTokens;
        }
    }
}
