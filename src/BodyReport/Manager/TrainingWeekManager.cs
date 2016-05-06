using BodyReport.Crud.Module;
using BodyReport.Models;
using Framework;
using Message;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    /// <summary>
    /// Manage training journal
    /// </summary>
    public class TrainingWeekManager : ServiceManager
    {
        TrainingWeekModule _trainingWeekModule = null;
        public TrainingWeekManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingWeekModule = new TrainingWeekModule(_dbContext);
        }
        
        internal TrainingWeek CreateTrainingWeek(TrainingWeek trainingWeek)
        {
            TrainingWeek trainingWeekResult = null;
            trainingWeekResult = _trainingWeekModule.Create(trainingWeek);

            if (trainingWeek.TrainingDays != null)
            {
                var trainingDayManager = new TrainingDayManager(_dbContext);
                trainingWeekResult.TrainingDays = new List<TrainingDay>();
                foreach (var trainingDay in trainingWeek.TrainingDays)
                {
                    trainingWeekResult.TrainingDays.Add(trainingDayManager.CreateTrainingDay(trainingDay));
                }
            }
            return trainingWeekResult;
        }

        internal TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek)
        {
            TrainingWeek trainingWeekResult = null;
            trainingWeekResult = _trainingWeekModule.Update(trainingWeek);

            if (trainingWeek.TrainingDays != null)
            {
                var trainingDayManager = new TrainingDayManager(_dbContext);
                trainingWeekResult.TrainingDays = new List<TrainingDay>();
                foreach (var trainingDay in trainingWeek.TrainingDays)
                {
                    trainingWeekResult.TrainingDays.Add(trainingDayManager.UpdateTrainingDay(trainingDay));
                }
            }
            return trainingWeekResult;
        }

        internal TrainingWeek GetTrainingWeek(TrainingWeekKey key, bool manageTrainingDay)
        {
            var trainingWeek = _trainingWeekModule.Get(key);
            if (trainingWeek != null && manageTrainingDay)
            {
                CompleteTrainingWeekWithTrainingDay(trainingWeek);
            }

            return trainingWeek;
        }

        private void CompleteTrainingWeekWithTrainingDay(TrainingWeek trainingWeek)
        {
            if (trainingWeek != null)
            {
                var trainingDayManager = new TrainingDayManager(_dbContext);
                var trainingDayCriteria = new TrainingDayCriteria()
                {
                    UserId = new StringCriteria() { EqualList = new List<string>() { trainingWeek.UserId } },
                    Year = new IntegerCriteria() { EqualList = new List<int>() { trainingWeek.Year } },
                    WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { trainingWeek.WeekOfYear } },
                };
                var trainingDayScenario = new TrainingDayScenario()
                {
                    ManageExercise = true
                };
                trainingWeek.TrainingDays = trainingDayManager.FindTrainingDay(trainingDayCriteria, trainingDayScenario);
            }
        }

        public List<TrainingWeek> FindTrainingWeek(CriteriaField criteriaField, TrainingWeekScenario trainingWeekScenario)
        {
            List<TrainingWeek> trainingWeeks = _trainingWeekModule.Find(criteriaField);
            
            if (trainingWeekScenario != null && trainingWeekScenario.ManageTrainingDay)
            {
                foreach (TrainingWeek trainingJournal in trainingWeeks)
                {
                    CompleteTrainingWeekWithTrainingDay(trainingJournal);
                }
            }

            return trainingWeeks;
        }

        internal void DeleteTrainingWeek(TrainingWeekKey key)
        {
            //TODO manage training Day and TrainingExercise
            var trainingWeek = GetTrainingWeek(key, true);
            if (trainingWeek != null)
            {
                _trainingWeekModule.Delete(key);

                if (trainingWeek.TrainingDays != null)
                {
                    var trainingDayManager = new TrainingDayManager(_dbContext);
                    foreach (var trainingDay in trainingWeek.TrainingDays)
                    {
                        trainingDayManager.DeleteTrainingDay(trainingDay);
                    }
                }
            }
        }
    }
}
