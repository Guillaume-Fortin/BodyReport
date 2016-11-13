using BodyReport.Crud.Module;
using BodyReport.Data;
using BodyReport.Models;
using BodyReport.Message;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Framework;
using BodyReport.ServiceLayers.Interfaces;
using BodyReport.ServiceLayers;

namespace BodyReport.Manager
{
    public class TrainingDayManager : BodyReportManager
    {
        TrainingDayModule _trainingDayModule = null;
        ITrainingExercisesService _trainingWeeksService = null;

        public TrainingDayManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingDayModule = new TrainingDayModule(DbContext);

            _trainingWeeksService = WebAppConfiguration.ServiceProvider.GetService<ITrainingExercisesService>();
            ((BodyReportService)_trainingWeeksService).SetDbContext(DbContext); // for use same transaction
        }

        internal TrainingDay CreateTrainingDay(TrainingDay trainingDay)
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
            var trainingDayList = FindTrainingDay(trainingDayCriteria, trainingDayScenario);
            int trainingDayId = 1;
            if (trainingDayList != null && trainingDayList.Count > 0)
            {
                trainingDayId = trainingDayList.Max(td => td.TrainingDayId) + 1;
            }

            trainingDay.TrainingDayId = trainingDayId;
            // no need transaction, only header

            TrainingDay trainingDayResult = null;
            trainingDayResult = _trainingDayModule.Create(trainingDay);
            SynchroManager.TrainingDayChange(DbContext, trainingDayResult);

            if (trainingDay.TrainingExercises != null)
            {
                trainingDayResult.TrainingExercises = new List<TrainingExercise>();
                foreach (var trainingExercise in trainingDay.TrainingExercises)
                {
                    trainingDayResult.TrainingExercises.Add(_trainingWeeksService.CreateTrainingExercise(trainingExercise));
                }
            }

            return trainingDayResult;
        }

        private void CompleteTrainingDayWithExercise(TrainingDay trainingJournalDay)
        {
            if (trainingJournalDay != null)
            {
                var trainingExerciseCriteria = new TrainingExerciseCriteria()
                {
                    UserId = new StringCriteria() { Equal = trainingJournalDay.UserId },
                    Year = new IntegerCriteria() { Equal = trainingJournalDay.Year },
                    WeekOfYear = new IntegerCriteria() { Equal = trainingJournalDay.WeekOfYear },
                    DayOfWeek = new IntegerCriteria() { Equal = trainingJournalDay.DayOfWeek },
                    TrainingDayId = new IntegerCriteria() { Equal = trainingJournalDay.TrainingDayId }
                };
                trainingJournalDay.TrainingExercises = _trainingWeeksService.FindTrainingExercise(trainingExerciseCriteria);
            }
        }

        internal TrainingDay GetTrainingDay(TrainingDayKey key, TrainingDayScenario trainingDayScenario)
        {
            var trainingDay = _trainingDayModule.Get(key);

            if (trainingDayScenario.ManageExercise && trainingDay != null)
            {
                CompleteTrainingDayWithExercise(trainingDay);
            }

            return trainingDay;
        }

        internal List<TrainingDay> FindTrainingDay(TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario)
        {
            var trainingDays = _trainingDayModule.Find(trainingDayCriteria);

            if (trainingDayScenario != null && trainingDayScenario.ManageExercise && trainingDays != null)
            {
                foreach (var trainingDay in trainingDays)
                {
                    CompleteTrainingDayWithExercise(trainingDay);
                }
            }

            return trainingDays;
        }

        internal TrainingDay UpdateTrainingDay(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario)
        {
            TrainingDay trainingDayResult = null;
            
            trainingDayResult = _trainingDayModule.Update(trainingDay);
            SynchroManager.TrainingDayChange(DbContext, trainingDayResult);

            if (trainingDayScenario != null && trainingDayScenario.ManageExercise)
            {
                var trainingExerciseCriteria = new TrainingExerciseCriteria()
                {
                    UserId = new StringCriteria() { Equal = trainingDay.UserId },
                    Year = new IntegerCriteria() { Equal = trainingDay.Year },
                    WeekOfYear = new IntegerCriteria() { Equal = trainingDay.WeekOfYear },
                    DayOfWeek = new IntegerCriteria() { Equal = trainingDay.DayOfWeek },
                    TrainingDayId = new IntegerCriteria() { Equal = trainingDay.TrainingDayId }
                };
                var trainingExercisesDb = _trainingWeeksService.FindTrainingExercise(trainingExerciseCriteria);
                if (trainingExercisesDb != null && trainingExercisesDb.Count > 0)
                {
                    foreach(var trainingExerciseDb in trainingExercisesDb)
                        _trainingWeeksService.DeleteTrainingExercise(trainingExerciseDb);
                }

                if (trainingDay.TrainingExercises != null)
                {
                    trainingDayResult.TrainingExercises = new List<TrainingExercise>();
                    foreach (var trainingExercise in trainingDay.TrainingExercises)
                    {
                        trainingDayResult.TrainingExercises.Add(_trainingWeeksService.UpdateTrainingExercise(trainingExercise, true));
                    }
                }
            }

            return trainingDayResult;
        }

        public void DeleteTrainingDay(TrainingDayKey key)
        {
            var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
            var trainingDay = GetTrainingDay(key, trainingDayScenario);
            if (trainingDay != null)
            {
                _trainingDayModule.Delete(trainingDay);
                SynchroManager.TrainingDayChange(DbContext, trainingDay, true);

                if (trainingDay.TrainingExercises != null)
                {
                    foreach (var trainingExercise in trainingDay.TrainingExercises)
                    {
                        _trainingWeeksService.DeleteTrainingExercise(trainingExercise);
                    }
                }
            }
        }
        
        private void DeleteTrainingDays(List<TrainingDay> trainingDays)
        {
            TrainingDay trainingDay;
            var trainingDayScenario = new TrainingDayScenario() { ManageExercise = true };
            if (trainingDays == null)
                return;
            foreach (var trainingDayKey in trainingDays)
            {
                trainingDay = GetTrainingDay(trainingDayKey, trainingDayScenario);
                if (trainingDay != null)
                    DeleteTrainingDay(trainingDay);
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

        public void SwitchDayOnTrainingDay(string userId, int year, int weekOfYear, int dayOfWeek, int switchDayOfWeek)
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
            var currentTrainingDayList = FindTrainingDay(trainingDayCriteria, trainingDayScenario);

            trainingDayCriteria.DayOfWeek.Equal = switchDayOfWeek;
            var switchTrainingDayList = FindTrainingDay(trainingDayCriteria, trainingDayScenario);

            DeleteTrainingDays(currentTrainingDayList);
            DeleteTrainingDays(switchTrainingDayList);

            if (currentTrainingDayList != null)
            {
                foreach (var trainingDay in currentTrainingDayList)
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
        }
    }
}
