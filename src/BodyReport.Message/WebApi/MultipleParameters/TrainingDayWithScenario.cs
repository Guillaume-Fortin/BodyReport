using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message.WebApi.MultipleParameters
{
    public class TrainingDayWithScenario
    {
        public TrainingDay TrainingDay { get; set; }
        public TrainingDayScenario TrainingDayScenario { get; set; }

        public TrainingDayWithScenario()
        { }
    }
}
