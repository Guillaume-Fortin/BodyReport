using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message.Web
{
    public class HttpException : Exception
    {
        public int Code { get; set; } = 0;

        public HttpException(string message, Exception innerException=null) : base(message, innerException)
        {
        }

        public HttpException(int code, string message, Exception innerException = null) : base(message, innerException)
        {
            Code = code;
        }
    }
}
