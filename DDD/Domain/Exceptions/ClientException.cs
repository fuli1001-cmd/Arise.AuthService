using System;
using System.Collections.Generic;
using System.Text;

namespace Arise.DDD.Domain.Exceptions
{
    /// <summary>
    /// 代表客户端错误
    /// </summary>
    public class ClientException : Exception
    {
        // 供开发人员使用的消息
        public List<string> DeveloperMessages { get; set; }

        public ClientException()
        { }

        public ClientException(string message)
            : base(message)
        { }

        public ClientException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public ClientException(string message, List<string> developerMessages)
            : base(message)
        {
            DeveloperMessages = developerMessages;
        }

        public ClientException(string message, List<string> developerMessages, Exception innerException)
            : base(message, innerException)
        {
            DeveloperMessages = developerMessages;
        }
    }
}
