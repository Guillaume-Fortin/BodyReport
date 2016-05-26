using BodyReport.Data;
using Framework;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public static class SynchroManager
    {
        public static void TrainingDayChange(ApplicationDbContext dbContext, TrainingDay trainingDay, bool deleted=false)
        {
            if (trainingDay == null || (!deleted && trainingDay.ModificationDate == null))
                return;

            var trainingWeekKey = new TrainingWeekKey()
            {
                UserId = trainingDay.UserId,
                Year = trainingDay.Year,
                WeekOfYear = trainingDay.WeekOfYear
            };
            var modificationDate = deleted ? Utils.DateTimeWithoutMs : trainingDay.ModificationDate;
            UpdateTrainingWeekModificationDate(dbContext, modificationDate, trainingWeekKey);
        }

        public static void TrainingExerciseChange(ApplicationDbContext dbContext, TrainingExercise trainingExercise, bool deleted = false)
        {
            if (trainingExercise == null || (!deleted && trainingExercise.ModificationDate == null))
                return;

            var trainingDayKey = new TrainingDayKey()
            {
                UserId = trainingExercise.UserId,
                Year = trainingExercise.Year,
                WeekOfYear = trainingExercise.WeekOfYear,
                DayOfWeek = trainingExercise.DayOfWeek,
                TrainingDayId = trainingExercise.TrainingDayId
            };
            var modificationDate = deleted ? Utils.DateTimeWithoutMs : trainingExercise.ModificationDate;
            UpdateTrainingDayModificationDate(dbContext, modificationDate, trainingDayKey);

            var trainingWeekKey = new TrainingWeekKey()
            {
                UserId = trainingExercise.UserId,
                Year = trainingExercise.Year,
                WeekOfYear = trainingExercise.WeekOfYear
            };
            UpdateTrainingWeekModificationDate(dbContext, modificationDate, trainingWeekKey);
        }

        private static void UpdateTrainingWeekModificationDate(ApplicationDbContext dbContext, DateTime modificationDate, TrainingWeekKey trainingWeekKey)
        {
            var trainingWeekManager = new TrainingWeekManager(dbContext);

            var scenario = new TrainingWeekScenario() { ManageTrainingDay = false };
            var trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, scenario);
            if (trainingWeek != null && trainingWeek.ModificationDate != null && modificationDate != null &&
               (modificationDate - trainingWeek.ModificationDate).TotalSeconds > 2) // don't spam database
            {
                trainingWeekManager.UpdateTrainingWeek(trainingWeek, scenario);
            }
        }

        private static void UpdateTrainingDayModificationDate(ApplicationDbContext dbContext, DateTime modificationDate, TrainingDayKey trainingDayKey)
        {
            var trainingDayManager = new TrainingDayManager(dbContext);
            
            var scenario = new TrainingDayScenario() { ManageExercise = false };
            var trainingDay = trainingDayManager.GetTrainingDay(trainingDayKey, scenario);
            if(trainingDay != null && trainingDay.ModificationDate != null && modificationDate != null &&
               (modificationDate - trainingDay.ModificationDate).TotalSeconds > 2) // don't spam database
            {
                trainingDayManager.UpdateTrainingDay(trainingDay, scenario);
            }
        }
    }
}
