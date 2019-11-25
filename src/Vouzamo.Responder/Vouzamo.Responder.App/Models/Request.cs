using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vouzamo.Responder.App.Models
{
    public class Request
    {
        public string Method { get; set; }
        public PathString Path { get; set; }
        public QueryString QueryString { get; set; }
        public string Body { get; set; }
        //public Dictionary<string, KeyValuePair<string, StringValues>> Headers { get; set; }

        public Request()
        {

        }
    }
}
