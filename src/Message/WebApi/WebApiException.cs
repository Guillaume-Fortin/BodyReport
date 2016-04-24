using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message.WebApi
{
    public class WebApiException : Exception
    {
        public int Code { get; set; } = 0;

        public WebApiException(string message, Exception innerException = null) : base(message, innerException)
        {
        }

        public WebApiException(int code, string message, Exception innerException = null) : base(message, innerException)
        {
            Code = code;
        }
    }
}
