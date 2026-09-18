using SmartQueue.Application.Exceptions;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace SmartQueue.API.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ExceptionHandler(context, ex);
            }
        }

        private static Task ExceptionHandler(HttpContext context, Exception ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = ex switch
            {
                NotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCodes = context.Response.StatusCode,
                Messages = ex.Message,
                Header = "Error recieved"
            }));
        }
    }
}
