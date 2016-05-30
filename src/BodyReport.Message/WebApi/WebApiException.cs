using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message.WebApi
{
    public class WebApiException : Exception
    {
        public int Code { get; set; } = 0;
        public new string Message { get; set; }
        public new Exception InnerException { get; set; }

        public WebApiException()
        {
        }

        public WebApiException(string message, Exception innerException = null) : base(message, innerException)
        {
            Message = message;
            InnerException = innerException;
        }

        public WebApiException(int code, string message, Exception innerException = null) : base(message, innerException)
        {
            Code = code;
            Message = message;
            InnerException = innerException;
        }
    }
}
