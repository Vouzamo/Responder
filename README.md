# Responder
ASP.NET Core API Proxy to help with specification based development (example specification: https://petstore.swagger.io/v2/swagger.json)

Anticipated to use the following:

## Server
ASP.NET Core using SignalR to facilitate client/server communication via websockets and OpenAPI.NET for strongly typed models of OpenAPI specification.

See:
* https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.0
* https://github.com/microsoft/OpenAPI.NET

## Client
React UI to initialize a SignalR hub connection for an OpenAPI specification and provide UI components to manage realtime or scripted responses against a temporary API proxy.

See:
* https://codingblast.com/asp-net-core-signalr-chat-react/
