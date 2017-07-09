using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface ITrainingDaysService
    {
        TrainingDay CreateTrainingDay(TrainingDay trainingDay);

        TrainingDay GetTrainingDay(TrainingDayKey key, TrainingDayScenario trainingDayScenario);

        List<TrainingDay> FindTrainingDay(TUnitType userUnit, TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario);

        TrainingDay UpdateTrainingDay(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario);

        void DeleteTrainingDay(TrainingDayKey key);

        void SwitchDayOnTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek, int switchDayOfWeek);

        void ChangeUnitForTrainingExercises(TrainingDay trainingDay, TUnitType oldUnit);
    }
}
