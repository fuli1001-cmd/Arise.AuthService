using Arise.DDD.API.Response;
using Arise.DDD.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Arise.DDD.API.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;

        public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // 客户端错误
            // 除了错误消息外，同时返回开发人员消息供开发人员使用
            if (context.Exception.GetType() == typeof(ClientException))
            {
                var clientException = context.Exception as ClientException;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Result = new ObjectResult(ResponseWrapper.CreateErrorResponseWrapper(
                    (StatusCode)context.HttpContext.Response.StatusCode,
                    clientException.Message,
                    clientException.DeveloperMessages));
            }
            else
            {
                // 服务端错误
                // 不返回服务器内部错误消息（DeveloperMessages）给客户端
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Result = new ObjectResult(ResponseWrapper.CreateErrorResponseWrapper((StatusCode)context.HttpContext.Response.StatusCode, context.Exception.Message));
            }

            context.ExceptionHandled = true;

            _logger.LogError("{@Exception}", context.Exception);
        }

        private class JsonErrorResponse
        {
            public string[] Messages { get; set; }
        }
    }
}
