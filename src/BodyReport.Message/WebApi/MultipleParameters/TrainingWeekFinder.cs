using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message.WebApi.MultipleParameters
{
    public class TrainingWeekFinder
    {
        public CriteriaList<TrainingWeekCriteria> TrainingWeekCriteriaList { get; set; }
        public TrainingWeekScenario TrainingWeekScenario { get; set; }

        public TrainingWeekFinder()
        { }
    }
}
