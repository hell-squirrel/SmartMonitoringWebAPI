using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using SmartMonitoring.Commons.Exceptions;
using System;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartMonitoring.Commons.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                LogException(exception);
                var response = context.Response;
                response.ContentType = "application/json";

                string responseText;
                switch (exception)
                {
                    case OperationException _:
                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                        responseText = exception?.Message;
                        break;
                    case SqliteException _:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseText = "Database execution exception.";
                        break;
                    default:
                        // unhandled error flow
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        responseText = exception?.Message;
                        break;
                }

                var responseMessage = JsonSerializer.Serialize(new { message = responseText });
                await response.WriteAsync(responseMessage);
            }
        }

        private void LogException(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine(exception?.Message);
            sb.AppendLine(exception?.StackTrace);
            _logger.LogError(sb.ToString());
        }
    }
}
