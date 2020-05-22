using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.API
{
    public class ResponseWrapper
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public static ResponseWrapper CreateOkResponseWrapper(object data)
        {
            return new ResponseWrapper { Data = data };
        }

        public static ResponseWrapper CreateErrorResponseWrapper(int code, string message)
        {
            return new ResponseWrapper { Code = code, Message = message };
        }

        public static ResponseWrapper CreateErrorResponseWrapper(int code, string message, object data)
        {
            return new ResponseWrapper { Code = code, Message = message, Data = data };
        }
    }
}
