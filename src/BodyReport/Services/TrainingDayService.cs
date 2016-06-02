using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Services
{
    public class TrainingDayService
    {
        ApplicationDbContext _dbContext = null;
        TrainingDayManager _trainingDayManager = null;

        public TrainingDayService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _trainingDayManager = new TrainingDayManager(_dbContext);
        }
        
        public TrainingDay CreateTrainingDay(TrainingDay trainingDay)
        {
            var trainingDayCriteria = new TrainingDayCriteria()
            {
                UserId = new StringCriteria() { Equal = trainingDay.UserId },
                Year = new IntegerCriteria() { Equal = trainingDay.Year },
                WeekOfYear = new IntegerCriteria() { Equal = trainingDay.WeekOfYear },
                DayOfWeek = new IntegerCriteria() { Equal = trainingDay.DayOfWeek },
            };
            var trainingDayScenario = new TrainingDayScenario()
            {
                ManageExercise = false
            };
            var trainingDayList = _trainingDayManager.FindTrainingDay(trainingDayCriteria, trainingDayScenario);
            int trainingDayId = 1;
            if (trainingDayList != null && trainingDayList.Count > 0)
            {
                trainingDayId = trainingDayList.Max(td => td.TrainingDayId) + 1;
            }

            trainingDay.TrainingDayId = trainingDayId;
            // no need transaction, only header
            return _trainingDayManager.CreateTrainingDay(trainingDay);
        }

        public TrainingDay UpdateTrainingDay(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario)
        {
            return _trainingDayManager.UpdateTrainingDay(trainingDay, trainingDayScenario);
        }

        public void DeleteTrainingDay(TrainingDayKey trainingDayKey)
        {
            var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
            var trainingDay = _trainingDayManager.GetTrainingDay(trainingDayKey, trainingDayScenario);
            if(trainingDay != null)
                _trainingDayManager.DeleteTrainingDay(trainingDay);
        }

        public void DeleteTrainingDay(TrainingDay trainingDay)
        {
            _trainingDayManager.DeleteTrainingDay(trainingDay);
        }

        public void DeleteTrainingDays(List<TrainingDay> trainingDays)
        {
            if (trainingDays == null)
                return;
            foreach (var trainingDay in trainingDays)
            {
                DeleteTrainingDay(trainingDay);
            }
        }

        public void SwitchDayOnTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek, int switchDayOfWeek)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };

                    //Search current training day
                    var trainingDayCriteria = new TrainingDayCriteria()
                    {
                        UserId = new StringCriteria() { Equal = userId },
                        Year = new IntegerCriteria() { Equal = year },
                        WeekOfYear = new IntegerCriteria() { Equal = weekOfYear },
                        DayOfWeek = new IntegerCriteria() { Equal = dayOfWeek }
                    };
                    var currentTrainingDayList = _trainingDayManager.FindTrainingDay(trainingDayCriteria, trainingDayScenario);

                    trainingDayCriteria.DayOfWeek.Equal = switchDayOfWeek;
                    var switchTrainingDayList = _trainingDayManager.FindTrainingDay(trainingDayCriteria, trainingDayScenario);

                    DeleteTrainingDays(currentTrainingDayList);
                    DeleteTrainingDays(switchTrainingDayList);

                    if (currentTrainingDayList != null)
                    {
                        foreach(var trainingDay in currentTrainingDayList)
                        {
                            ChangeDayOnTrainingDay(trainingDay, switchDayOfWeek);
                            UpdateTrainingDay(trainingDay, trainingDayScenario);
                        }
                    }

                    if (switchTrainingDayList != null)
                    {
                        foreach (var trainingDay in switchTrainingDayList)
                        {
                            ChangeDayOnTrainingDay(trainingDay, dayOfWeek);
                            UpdateTrainingDay(trainingDay, trainingDayScenario);
                        }
                    }

                    transaction.Commit();
                }
                catch(Exception except)
                {
                    //_logger.LogCritical("Unable to delete training week", exception);
                    transaction.Rollback();
                    throw except;
                }
            }
        }

        private void ChangeDayOnTrainingDay(TrainingDay trainingDay, int newDayOfWeek)
        {
            if (trainingDay == null)
                return;

            trainingDay.DayOfWeek = newDayOfWeek;
            if (trainingDay.TrainingExercises != null)
            {
                foreach (var trainingExercise in trainingDay.TrainingExercises)
                {
                    trainingExercise.DayOfWeek = newDayOfWeek;
                    if (trainingExercise.TrainingExerciseSets != null)
                    {
                        foreach (var set in trainingExercise.TrainingExerciseSets)
                        {
                            set.DayOfWeek = newDayOfWeek;
                        }
                    }
                }
            }
        }
    }
}
