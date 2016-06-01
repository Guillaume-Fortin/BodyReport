using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message.WebApi.MultipleParameters
{
    public class SwitchDayParameter
    {
        public TrainingDayKey TrainingDayKey { get; set; }
        public int SwitchDayOfWeek { get; set; }

        public SwitchDayParameter()
        {
        }
    }
}
