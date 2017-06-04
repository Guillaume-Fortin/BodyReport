using System;
using System.Collections.Generic;
using System.Text;

namespace BodyReport.Message
{
    public class MailTestResult
    {
        /// <summary>
        /// Test Result
        /// </summary>
        public bool Result { get; set; } = false;
        /// <summary>
        /// Mail data
        /// </summary>
        public string MailData { get; set; } = string.Empty;
    }
}
