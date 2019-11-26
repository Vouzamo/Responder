using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;
using Vouzamo.Responder.App.Models;

namespace Vouzamo.Responder.App.Results
{
    public class JobResponseActionResult : IActionResult
    {
        protected Response Response { get; }

        public JobResponseActionResult(Response response)
        {
            Response = response;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = context.HttpContext.Response;

            response.StatusCode = Response.StatusCode;

            if(!string.IsNullOrEmpty(Response.Body))
            {
                response.ContentType = Response.ContentType;

                var content = Encoding.UTF8.GetBytes(Response.Body);
                await response.Body.WriteAsync(content, 0, content.Length);
            }
        }
    }
}
