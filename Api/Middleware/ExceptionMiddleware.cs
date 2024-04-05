using Application.Exceptions;
using System.Net;
using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode;
            string result = JsonConvert.SerializeObject(new ErrorDetails 
            { 
                ErrorMessage = exception.Message, 
                ErrorType = "Failure" 
            });

            switch (exception)
            {
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(validationException.Errors);
                    break;
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case AssignRoleException assignRoleException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case LoginException loginException:
                    statusCode = HttpStatusCode.Unauthorized;
                    break;
                case RegisterException registerException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case DbException dbException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case ImageUploadException imageUploadException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case CreateRoleException createRoleException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case DbUpdateException dbUpdateException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(result);
        }
    }

    public class ErrorDetails
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
