using BodyReport.Framework.Exceptions;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Resources;
using Message;
using Message.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace BodyReport.Services
{
    public class TrainingWeekService
    {
        ApplicationDbContext _dbContext = null;

        public TrainingWeekService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private void ChangeIDForNewTrainingWeek(TrainingWeek trainingWeek, int oldTrainingYear, int oldTrainingWeek, int newTrainingYear, int newTrainingWeek)
        {
            trainingWeek.Year = newTrainingYear;
            trainingWeek.WeekOfYear = newTrainingWeek;
            if (trainingWeek.TrainingDays != null)
            {
                foreach (var trainginDay in trainingWeek.TrainingDays)
                {
                    trainginDay.Year = newTrainingYear;
                    trainginDay.WeekOfYear = newTrainingWeek;

                    if (trainginDay.TrainingExercises != null)
                    {
                        foreach (var trainginsExercise in trainginDay.TrainingExercises)
                        {
                            trainginsExercise.Year = newTrainingYear;
                            trainginsExercise.WeekOfYear = newTrainingWeek;

                            if (trainginsExercise.TrainingExerciseSets != null)
                            {
                                foreach (var set in trainginsExercise.TrainingExerciseSets)
                                {
                                    set.Year = newTrainingYear;
                                    set.WeekOfYear = newTrainingWeek;
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool CopyTrainingWeek(string currentUserId, CopyTrainingWeek copyTrainingWeek, out TrainingWeek newTrainingWeek)
        {
            newTrainingWeek = null;
                if (string.IsNullOrWhiteSpace(copyTrainingWeek.UserId) || copyTrainingWeek.OriginYear == 0 || copyTrainingWeek.OriginWeekOfYear == 0 ||
                copyTrainingWeek.Year == 0 || copyTrainingWeek.WeekOfYear == 0 || currentUserId != copyTrainingWeek.UserId)
                return false;

            bool result = false;
            //Verify valid week of year
            if (copyTrainingWeek.WeekOfYear > 0 && copyTrainingWeek.WeekOfYear <= 52 &&
                !(copyTrainingWeek.Year == copyTrainingWeek.OriginYear && copyTrainingWeek.WeekOfYear == copyTrainingWeek.OriginWeekOfYear))
            {
                var trainingWeekManager = new TrainingWeekManager(_dbContext);
                //check if new trainingWeek exist
                var trainingWeekKey = new TrainingWeekKey() { UserId = copyTrainingWeek.UserId, Year = copyTrainingWeek.Year, WeekOfYear = copyTrainingWeek.WeekOfYear };
                var trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, true);

                if (trainingWeek != null)
                    throw new ErrorException(string.Format(Translation.P0_ALREADY_EXIST, Translation.TRAINING_WEEK));

                // Check if origin training week exist
                trainingWeekKey = new TrainingWeekKey() { UserId = copyTrainingWeek.UserId, Year = copyTrainingWeek.OriginYear, WeekOfYear = copyTrainingWeek.OriginWeekOfYear };
                trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, true);

                if (trainingWeek == null)
                    throw new ErrorException(string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_WEEK));

                //Change ids of origin training week/exercise/day etc.. for new trainingweek
                ChangeIDForNewTrainingWeek(trainingWeek, copyTrainingWeek.OriginYear, copyTrainingWeek.OriginWeekOfYear, copyTrainingWeek.Year, copyTrainingWeek.WeekOfYear);

                // Create data in database (with update for Security existing old data in database)
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        trainingWeek = trainingWeekManager.UpdateTrainingWeek(trainingWeek);
                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        //_logger.LogCritical("Unable to copy training week", exception);
                        transaction.Rollback();
                        throw exception;
                    }
                }

                if (trainingWeek == null)
                {
                    throw new ErrorException(string.Format(Translation.IMPOSSIBLE_TO_CREATE_P0, Translation.TRAINING_WEEK));
                }
                else
                {
                    result = true;
                    newTrainingWeek = trainingWeek;
                }
            }
            return result;
        }
    }
}
