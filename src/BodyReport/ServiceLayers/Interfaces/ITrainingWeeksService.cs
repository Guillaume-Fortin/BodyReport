using BodyReport.Message;
using BodyReport.Message.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface ITrainingWeeksService
    {
        TrainingWeek CreateTrainingWeek(TrainingWeek trainingWeek);

        TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek, TrainingWeekScenario trainingWeekScenario);

        TrainingWeek GetTrainingWeek(TrainingWeekKey key, TrainingWeekScenario trainingWeekScenario);

        List<TrainingWeek> FindTrainingWeek(TrainingWeekCriteria trainingWeekCriteria, TrainingWeekScenario trainingWeekScenario);

        List<TrainingWeek> FindTrainingWeek(CriteriaList<TrainingWeekCriteria> trainingWeekCriteriaList, TrainingWeekScenario trainingWeekScenario);

        void DeleteTrainingWeek(TrainingWeekKey key);
        bool CopyTrainingWeek(string currentUserId, CopyTrainingWeek copyTrainingWeek, out TrainingWeek newTrainingWeek);
    }
}
