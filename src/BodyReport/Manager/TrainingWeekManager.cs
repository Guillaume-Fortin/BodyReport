using BodyReport.Crud.Module;
using BodyReport.Data;
using BodyReport.Models;
using BodyReport.Framework;
using BodyReport.Message;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Message.Web;
using BodyReport.Framework.Exceptions;
using BodyReport.Resources;

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

            if (trainingWeekScenario != null && trainingWeekScenario.ManageTrainingDay)
            {
                var trainingDayManager = new TrainingDayManager(_dbContext);
                var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };

                var trainingDayCriteria = new TrainingDayCriteria()
                {
                    UserId = new StringCriteria() { Equal = trainingWeek.UserId },
                    Year = new IntegerCriteria() { Equal = trainingWeek.Year },
                    WeekOfYear = new IntegerCriteria() { Equal = trainingWeek.WeekOfYear }
                };
                var trainingDaysDb = trainingDayManager.FindTrainingDay(trainingDayCriteria, trainingDayScenario);
                if (trainingDaysDb != null && trainingDaysDb.Count > 0)
                {
                    foreach (var trainingDayDb in trainingDaysDb)
                        trainingDayManager.DeleteTrainingDay(trainingDayDb);
                }

                if (trainingWeek.TrainingDays != null)
                {
                    trainingWeekResult.TrainingDays = new List<TrainingDay>();
                    foreach (var trainingDay in trainingWeek.TrainingDays)
                    {
                        trainingWeekResult.TrainingDays.Add(trainingDayManager.UpdateTrainingDay(trainingDay, trainingWeekScenario.TrainingDayScenario));
                    }
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
            return FindTrainingWeek(new CriteriaList<TrainingWeekCriteria>() { trainingWeekCriteria }, trainingWeekScenario);
        }

        public List<TrainingWeek> FindTrainingWeek(CriteriaList<TrainingWeekCriteria> trainingWeekCriteriaList, TrainingWeekScenario trainingWeekScenario)
        {
            List<TrainingWeek> trainingWeeks = _trainingWeekModule.Find(trainingWeekCriteriaList);
            
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
                var trainingWeekScenario = new TrainingWeekScenario() { ManageTrainingDay = false };
                var trainingWeekKey = new TrainingWeekKey() { UserId = copyTrainingWeek.UserId, Year = copyTrainingWeek.Year, WeekOfYear = copyTrainingWeek.WeekOfYear };
                var trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, trainingWeekScenario);

                if (trainingWeek != null)
                    throw new ErrorException(string.Format(Translation.P0_ALREADY_EXIST, Translation.TRAINING_WEEK));

                // Check if origin training week exist
                trainingWeekScenario = new TrainingWeekScenario()
                {
                    ManageTrainingDay = true,
                    TrainingDayScenario = new TrainingDayScenario() { ManageExercise = true }
                };
                trainingWeekKey = new TrainingWeekKey() { UserId = copyTrainingWeek.UserId, Year = copyTrainingWeek.OriginYear, WeekOfYear = copyTrainingWeek.OriginWeekOfYear };
                trainingWeek = trainingWeekManager.GetTrainingWeek(trainingWeekKey, trainingWeekScenario);

                if (trainingWeek == null)
                    throw new ErrorException(string.Format(Translation.P0_NOT_EXIST, Translation.TRAINING_WEEK));

                //Change ids of origin training week/exercise/day etc.. for new trainingweek
                ChangeIDForNewTrainingWeek(trainingWeek, copyTrainingWeek.OriginYear, copyTrainingWeek.OriginWeekOfYear, copyTrainingWeek.Year, copyTrainingWeek.WeekOfYear);

                // Create data in database (with update for Security existing old data in database)
                trainingWeek = trainingWeekManager.UpdateTrainingWeek(trainingWeek, trainingWeekScenario);

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
