using BodyReport.Crud.Module;
using BodyReport.Models;
using Message;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class TrainingDayManager : ServiceManager
    {
        TrainingDayModule _trainingDayModule = null;

        public TrainingDayManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingDayModule = new TrainingDayModule(_dbContext);
        }

        internal TrainingDay CreateTrainingDay(TrainingDay trainingDay, bool manageTransaction = true)
        {
            TrainingDay trainingDayResult = null;
            IRelationalTransaction transaction = null;
            if (manageTransaction)
                transaction = _dbContext.Database.BeginTransaction();

            try
            {
                trainingDayResult = _trainingDayModule.Create(trainingDay);

                if (trainingDayResult.TrainingExercises != null)
                {
                    var trainingExerciseManager = new TrainingExerciseManager(_dbContext);
                    foreach (var trainingExercise in trainingDayResult.TrainingExercises)
                    {
                        trainingDayResult.TrainingExercises.Add(trainingExerciseManager.CreateTrainingExercise(trainingExercise));
                    }
                }

                if (manageTransaction)
                    transaction.Commit();
            }
            catch (Exception exception)
            {
                if (manageTransaction)
                    transaction.Rollback();
                throw exception;
            }
            finally
            {
                if (manageTransaction)
                    transaction.Dispose();
            }
            return trainingDayResult;
        }

        private void CompleteTrainingDayWithExercise(TrainingDay trainingJournalDay)
        {
            if (trainingJournalDay != null)
            {
                var trainingExerciseCriteria = new TrainingDayExerciseCriteria()
                {
                    UserId = new StringCriteria() { EqualList = new List<string>() { trainingJournalDay.UserId } },
                    Year = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.Year } },
                    WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.Year } },
                    DayOfWeek = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.DayOfWeek } },
                    TrainingDayId = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.TrainingDayId } }
                };
                var trainingExerciseManager = new TrainingExerciseManager(_dbContext);
                trainingJournalDay.TrainingExercises = trainingExerciseManager.FindTrainingExercise(trainingExerciseCriteria);
            }
        }

        internal TrainingDay GetTrainingDay(TrainingDayKey key, bool manageExercise)
        {
            var trainingDay = _trainingDayModule.Get(key);

            if (manageExercise && trainingDay != null)
            {
                CompleteTrainingDayWithExercise(trainingDay);
            }

            return trainingDay;
        }

        internal List<TrainingDay> FindTrainingDay(CriteriaField criteriaField, bool manageExercise)
        {
            var trainingDays = _trainingDayModule.Find(criteriaField);

            if (manageExercise && trainingDays != null)
            {
                foreach (var trainingDay in trainingDays)
                {
                    CompleteTrainingDayWithExercise(trainingDay);
                }
            }

            return trainingDays;
        }

        internal TrainingDay UpdateTrainingDay(TrainingDay trainingDay, bool manageTransaction = true)
        {
            TrainingDay trainingDayResult = null;
            IRelationalTransaction transaction = null;
            if (manageTransaction)
                transaction = _dbContext.Database.BeginTransaction();

            try
            {
                trainingDayResult = _trainingDayModule.Update(trainingDay);

                if (trainingDay.TrainingExercises != null)
                {
                    var trainingExerciseManager = new TrainingExerciseManager(_dbContext);
                    foreach (var trainingExercise in trainingDay.TrainingExercises)
                    {
                        trainingDayResult.TrainingExercises.Add(trainingExerciseManager.UpdateTrainingExercise(trainingExercise));
                    }
                }

                if (manageTransaction)
                    transaction.Commit();
            }
            catch (Exception exception)
            {
                if (manageTransaction)
                    transaction.Rollback();
                throw exception;
            }
            finally
            {
                if (manageTransaction)
                    transaction.Dispose();
            }
            return trainingDayResult;
        }

        internal void DeleteTrainingDay(TrainingDay trainingDay, bool manageTransaction = true)
        {
            IRelationalTransaction transaction = null;
            if (manageTransaction)
                transaction = _dbContext.Database.BeginTransaction();
            
            try
            {
                _trainingDayModule.Delete(trainingDay);

                if (trainingDay.TrainingExercises != null)
                {
                    var trainingExerciseManager = new TrainingExerciseManager(_dbContext);
                    foreach (var trainingExercise in trainingDay.TrainingExercises)
                    {
                        trainingExerciseManager.DeleteTrainingExercise(trainingExercise);
                    }
                }

                if (manageTransaction)
                    transaction.Commit();
            }
            catch (Exception exception)
            {
                if (manageTransaction)
                    transaction.Rollback();
                throw exception;
            }
            finally
            {
                if (manageTransaction)
                    transaction.Dispose();
            }
        }
    }
}
