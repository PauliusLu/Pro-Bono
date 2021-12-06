using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Karma.Middleware
{
    public class AnalyticsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public AnalyticsMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            ControllerActionDescriptor descriptor = getDescriptor(context);
            string controller = descriptor?.ControllerName;
            string action = descriptor?.ActionName;
            
            string log = formatLogMessage(context.User.Identity.Name,
                                          context.User.Identity.IsAuthenticated.ToString(),
                                          context.Connection.RemoteIpAddress.ToString(),
                                          context.Request.Path,
                                          controller, action);

            try
            {
                await _next(context);
            }
            catch(Exception e)
            {
                _logger.Error(e, $"Failure: {log}\n");
                await HandleException(context);
            }

            // Avoid logging multiple message GET requests
            if (action == "Index" && controller == "Messages")
                return ;

            _logger.Information(log);
        }

        private Task HandleException(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            string errorType = "Something is wrong with the website";

            return context.Response.WriteAsync(new {Message = errorType}.ToString());
        }

        public ControllerActionDescriptor getDescriptor(HttpContext context)
        {
            return context?
                .GetEndpoint()?
                .Metadata?
                .GetMetadata<ControllerActionDescriptor>();
        }

        public string formatLogMessage(string user, string isAuth, string ip, string path, string? controller, string? action)
        {
            string controllerString = (controller == null) ? "" : $"\tController: {controller}\n";
            string actionString = (action == null) ? "" : $"\tAction: {action}\n";

            return $"\n\tUsername: {user}\n" +
                   $"\tIs authenticated: {isAuth}\n" +
                   $"\tIP address: {ip}\n" +
                   $"\tPath: {path}\n" +
                   controllerString +
                   actionString;

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AnalyticsMiddlewareExtensions
    {
        public static IApplicationBuilder UseAnalyticsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AnalyticsMiddleware>();
        }
    }
}
