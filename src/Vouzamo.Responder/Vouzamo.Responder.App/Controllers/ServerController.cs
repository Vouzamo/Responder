using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Vouzamo.Responder.App.Controllers
{
    [Route("api/server")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        [Route("create-api")]
        public async Task<ActionResult> CreateApi(string uri)
        {
            using var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(uri)
            };

            var response = await httpClient.GetAsync(string.Empty);
            var json = await response.Content.ReadAsStringAsync();

            var reader = new Microsoft.OpenApi.Readers.OpenApiStringReader();

            var document = reader.Read(json, out var diagnostic);

            return Ok(document.Info.Title);
        }
    }
}