using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Framework.Exceptions
{
    public class ErrorException : Exception
    {
        public ErrorException(string message) : base(message)
        {

        }
    }
}
