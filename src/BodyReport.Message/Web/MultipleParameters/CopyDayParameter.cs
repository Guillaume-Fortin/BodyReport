using System;
using System.Collections.Generic;
using System.Text;

namespace BodyReport.Message.Web.MultipleParameters
{
    public class CopyDayParameter
    {
        public TrainingDayKey TrainingDayKey { get; set; }
        public int CopyDayOfWeek { get; set; }

        public CopyDayParameter()
        {
        }
    }
}
