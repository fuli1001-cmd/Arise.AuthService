using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.API.Response
{
    public class ResponseWrapper
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        protected ResponseWrapper(object data)
        {
            Data = data;
        }

        protected ResponseWrapper(int code, string message)
        {
            Code = code;
            Message = message;
        }

        protected ResponseWrapper(int code, string message, object data)
        {
            Code = code;
            Message = message;
            Data = data;
        }

        public static ResponseWrapper CreateOkResponseWrapper(object data)
        {
            return new ResponseWrapper(data);
        }

        public static ResponseWrapper CreateErrorResponseWrapper(int code, string message)
        {
            return new ResponseWrapper(code, message);
        }

        public static ResponseWrapper CreateErrorResponseWrapper(int code, string message, object data)
        {
            return new ResponseWrapper(code, message, data);
        }
    }
}
