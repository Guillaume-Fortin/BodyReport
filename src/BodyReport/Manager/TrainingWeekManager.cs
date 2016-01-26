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
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    trainingWeekResult = _trainingWeekModule.Create(trainingWeek);

                    if (trainingWeek.TrainingDays != null)
                    {
                        var trainingDayManager = new TrainingDayManager(_dbContext);
                        foreach(var trainingDay in trainingWeek.TrainingDays)
                        {
                            trainingWeekResult.TrainingDays.Add(trainingDayManager.CreateTrainingDay(trainingDay, false));
                        }
                    }
                    transaction.Commit();
                }
                catch(Exception exception)
                {
                    transaction.Rollback();
                    throw exception;
                }
            }
            return trainingWeekResult;
        }

        internal TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek)
        {
            TrainingWeek trainingWeekResult = null;
            using (_dbContext.Database.BeginTransaction())
            {
                try
                {
                    trainingWeekResult = _trainingWeekModule.Update(trainingWeek);

                    if (trainingWeek.TrainingDays != null)
                    {
                        var trainingDayManager = new TrainingDayManager(_dbContext);
                        foreach (var trainingDay in trainingWeek.TrainingDays)
                        {
                            trainingWeekResult.TrainingDays.Add(trainingDayManager.UpdateTrainingDay(trainingDay, false));
                        }
                    }
                    _dbContext.Database.CommitTransaction();
                }
                catch (Exception exception)
                {
                    _dbContext.Database.RollbackTransaction();
                    throw exception;
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
                trainingWeek.TrainingDays = trainingDayManager.FindTrainingDay(trainingDayCriteria, true);
            }
        }

        public List<TrainingWeek> FindTrainingWeek(CriteriaField criteriaField, bool manageTrainingDay)
        {
            List<TrainingWeek> trainingWeeks = _trainingWeekModule.Find(criteriaField);
            
            if (manageTrainingDay)
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
            using (_dbContext.Database.BeginTransaction())
            {
                try
                {
                    var trainingWeek = GetTrainingWeek(key, true);
                    if (trainingWeek != null)
                    {
                        _trainingWeekModule.Delete(key);

                        if (trainingWeek.TrainingDays != null)
                        {
                            var trainingDayManager = new TrainingDayManager(_dbContext);
                            foreach (var trainingDay in trainingWeek.TrainingDays)
                            {
                                trainingDayManager.DeleteTrainingDay(trainingDay, false);
                            }
                        }
                        _dbContext.Database.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    _dbContext.Database.RollbackTransaction();
                    throw exception;
                }
            }
        }
    }
}
