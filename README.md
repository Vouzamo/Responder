# Responder
ASP.NET Core API Proxy to help with specification based development (example specification: https://petstore.swagger.io/v2/swagger.json)

Anticipated to use the following:

## Server
ASP.NET Core using SignalR to facilitate client/server communication via websockets and OpenAPI.NET for strongly typed models of OpenAPI specification.

See:
* https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-3.0
* https://github.com/microsoft/OpenAPI.NET

### Proxy
Proxy endpoint will be in the format /proxy/{workspace}/{*uri} and will listen for any http requests and submit them as a job for the workspace.

### Api
Api endpoints will facilitate paginating through jobs and completing a job by providing a response or rule to apply for it.

## Client
React UI to initialize a SignalR hub connection for an OpenAPI specification and provide UI components to manage realtime or scripted responses.

See:
* https://codingblast.com/asp-net-core-signalr-chat-react/

### Realtime
See workspace jobs and use the UI to repond to them either by creating a response manually, or selecting a rule to apply.

### Rules
Manage rules for a workspace. Also includes the capability to use an OpenAPI specification to evaluate a request and respond accordingly or to generate a rule for each endpoint defined in an OpenAPI specification that can then be customized further.

### Settings
Workspace settings to choose between modes of automation:
* Intercept All - Always intercept requests and require manual confirmation of response
* Intercept Unmatched - Only intercept requests that can't be matched to a rule based on evaluating conditions.
* Intercept None - Instead of intercepting you can specify a default response to use for any unmatched requests.
