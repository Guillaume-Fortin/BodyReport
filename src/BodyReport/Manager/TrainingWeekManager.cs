using BodyReport.Crud.Module;
using BodyReport.Data;
using BodyReport.Models;
using Framework;
using Message;
using Microsoft.EntityFrameworkCore;
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

        internal TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek, TrainingWeekScenario trainingWeekScenario)
        {
            TrainingWeek trainingWeekResult = null;
            trainingWeekResult = _trainingWeekModule.Update(trainingWeek);

            if (trainingWeekScenario != null && trainingWeekScenario.ManageTrainingDay && trainingWeek.TrainingDays != null)
            {
                var trainingDayManager = new TrainingDayManager(_dbContext);
                trainingWeekResult.TrainingDays = new List<TrainingDay>();
                foreach (var trainingDay in trainingWeek.TrainingDays)
                {
                    trainingWeekResult.TrainingDays.Add(trainingDayManager.UpdateTrainingDay(trainingDay, trainingWeekScenario.TrainingDayScenario));
                }
            }
            return trainingWeekResult;
        }

        internal TrainingWeek GetTrainingWeek(TrainingWeekKey key, TrainingWeekScenario trainingWeekScenario)
        {
            var trainingWeek = _trainingWeekModule.Get(key);
            if (trainingWeek != null && trainingWeekScenario != null && trainingWeekScenario.ManageTrainingDay)
            {
                CompleteTrainingWeekWithTrainingDay(trainingWeek, trainingWeekScenario.TrainingDayScenario);
            }

            return trainingWeek;
        }

        private void CompleteTrainingWeekWithTrainingDay(TrainingWeek trainingWeek, TrainingDayScenario trainingDayScenario)
        {
            if (trainingWeek != null)
            {
                var trainingDayManager = new TrainingDayManager(_dbContext);
                var trainingDayCriteria = new TrainingDayCriteria()
                {
                    UserId = new StringCriteria() { Equal = trainingWeek.UserId },
                    Year = new IntegerCriteria() { Equal = trainingWeek.Year },
                    WeekOfYear = new IntegerCriteria() { Equal = trainingWeek.WeekOfYear },
                };
                trainingWeek.TrainingDays = trainingDayManager.FindTrainingDay(trainingDayCriteria, trainingDayScenario);
            }
        }

        public List<TrainingWeek> FindTrainingWeek(TrainingWeekCriteria trainingWeekCriteria, TrainingWeekScenario trainingWeekScenario)
        {
            List<TrainingWeek> trainingWeeks = _trainingWeekModule.Find(trainingWeekCriteria);
            
            if (trainingWeekScenario != null && trainingWeekScenario.ManageTrainingDay)
            {
                foreach (TrainingWeek trainingJournal in trainingWeeks)
                {
                    CompleteTrainingWeekWithTrainingDay(trainingJournal, trainingWeekScenario.TrainingDayScenario);
                }
            }

            return trainingWeeks;
        }

        internal void DeleteTrainingWeek(TrainingWeekKey key)
        {
            var trainingWeekScenario = new TrainingWeekScenario()
            {
                ManageTrainingDay = true,
                TrainingDayScenario = new TrainingDayScenario() { ManageExercise = true }
            };
            var trainingWeek = GetTrainingWeek(key, trainingWeekScenario);
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
