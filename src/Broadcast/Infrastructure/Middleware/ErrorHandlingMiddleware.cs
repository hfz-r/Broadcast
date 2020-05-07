using System;
using System.Net;
using System.Threading.Tasks;
using Broadcast.Core;
using Broadcast.Core.Infrastructure;
using Broadcast.Core.Infrastructure.Errors;
using Broadcast.Services.Logging;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Broadcast.Infrastructure.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(
            ILogger logger,
            RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            ILogger logger)
        {
            object errors = null;

            switch (exception)
            {
                case RestException re:
                    errors = re.Errors;
                    context.Response.StatusCode = (int) re.Code;
                    break;
                case Exception e:
                    errors = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";

            var result = JsonConvert.SerializeObject(new
            {
                errors
            });

            var currentUser = EngineContext.Current?.Resolve<ICurrentUserAccessor>();
            await logger.ErrorAsync(errors?.ToString(), exception, currentUser?.CurrentUser);

            await context.Response.WriteAsync(result);
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }
    }
}