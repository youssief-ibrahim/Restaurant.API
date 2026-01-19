using System.Net;
using Restaurant.Domain.Exceptions;

namespace Restaurant.API.Middlewares.ExceptionMid
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await Handle(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (BadHttpRequestException ex)
            {
                await Handle(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (ForbidException ex)
            {
                await Handle(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (Exception ex)
            {
                await Handle(context, HttpStatusCode.InternalServerError, ex.Message.ToString());
            }
        }

        private static async Task Handle(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(message);
        }
    }
}
