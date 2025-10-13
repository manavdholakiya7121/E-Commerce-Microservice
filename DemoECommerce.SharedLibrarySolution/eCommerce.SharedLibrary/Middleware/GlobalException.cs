using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "sorry, internal server error occured. Kidly try again";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    message = "Too many requests";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    title = "Warning";
                    await ModifyHeader(context, title, message, statusCode);
                }

                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    title = "Alert";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    message = "You are not authorized to access";
                    await ModifyHeader(context, title, message, statusCode);
                }

                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    title = "Out of Access";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    message = "You are not allowed to access";
                    await ModifyHeader(context, title, message, statusCode);
                }

            }
            catch (Exception ex) 
            { 
                LogException.LogError(ex);

                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of Time";
                    statusCode = (int)StatusCodes.Status408RequestTimeout;
                    message = "Request Timeout";                   
                }

                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Title = title,
                Detail = message,
                Status = statusCode,
                
            }), CancellationToken.None);
            return; 
        }
    }
}
