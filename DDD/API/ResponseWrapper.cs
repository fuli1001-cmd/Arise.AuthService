using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.API
{
    public class ResponseWrapper
    {
        public int Code { get; set; }
        public string[] Messages { get; set; }
        public object Data { get; set; }

        public static ResponseWrapper CreateOkResponseWrapper(object data)
        {
            return new ResponseWrapper { Data = data };
        }

        public static ResponseWrapper CreateErrorResponseWrapper(int code, string[] messages)
        {
            return new ResponseWrapper { Code = code, Messages = messages };
        }

        public static ResponseWrapper CreateErrorResponseWrapper(int code, string[] messages, object data)
        {
            return new ResponseWrapper { Code = code, Messages = messages, Data = data };
        }
    }
}
