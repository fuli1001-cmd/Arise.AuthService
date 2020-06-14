using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.API.Response
{
    public class ResponseWrapper
    {
        // 状态码
        public StatusCode Code { get; set; }

        // 显示给用户的消息
        public string Message { get; set; }

        // 开发人员使用的消息
        public List<string> DeveloperMessages { get; set; }

        public object Data { get; set; }

        protected ResponseWrapper(object data)
        {
            Code = StatusCode.OK;
            Data = data;
        }

        protected ResponseWrapper(StatusCode code, string message, List<string> developerMessages)
        {
            Code = code;
            Message = message;
            DeveloperMessages = developerMessages;
        }

        protected ResponseWrapper(StatusCode code, string message, List<string> developerMessages, object data)
            : this(code, message, developerMessages)
        {
            Data = data;
        }

        public static ResponseWrapper CreateOkResponseWrapper(object data)
        {
            return new ResponseWrapper(data);
        }

        public static ResponseWrapper CreateErrorResponseWrapper(StatusCode code, string message, List<string> developerMessages = null)
        {
            return new ResponseWrapper(code, message, developerMessages);
        }

        public static ResponseWrapper CreateErrorResponseWrapper(StatusCode code, string message, List<string> developerMessages, object data)
        {
            return new ResponseWrapper(code, message, developerMessages, data);
        }
    }

    // 错误状态码
    // reference: https://blog.restcase.com/rest-api-error-codes-101/
    public enum StatusCode
    {
        OK = 0, // 正常
        ClientError = 400, // 客户端错误
        Unauthorized = 401, // 未授权
        ServerError = 500 // 服务端错误
    }
}
