using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Errands.Users.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Errands.Users.Api
{
    public class ExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                if (context.Exception is BusinessException bizEx)
                {
                    context.Result = new BadRequestObjectResult(bizEx.Message);
                    if (bizEx.LogData != null)
                    {
                        GetLogger(context).LogError(JsonSerializer.Serialize(bizEx.LogData));
                    }
                }
                else
                {
                    var message = GetInnerMostMessage(context.Exception) ?? "Oops! Something went wrong.";
                    var request = context.HttpContext.Request;
                    context.Result = new ObjectResult(JsonSerializer.Serialize(new
                    {
                        message,
                        request.Path,
                        request.QueryString,
                        context.Exception.StackTrace
                    }))
                    {
                        StatusCode = 500
                    };

                    GetLogger(context).LogError(JsonSerializer.Serialize(new
                    {
                        message,
                        request.Path,
                        request.QueryString,
                        context.Exception.StackTrace
                    }));
                }
                context.ExceptionHandled = true;
            }
        }

        private string GetInnerMostMessage(Exception ex)
        {
            if (ex.InnerException == null)
                return ex.Message;
            return GetInnerMostMessage(ex.InnerException);
        }

        private static ILogger GetLogger(ActionExecutedContext context)
        {
            return context.HttpContext.RequestServices.GetService<ILogger<ExceptionFilter>>();
        }
    }
}
