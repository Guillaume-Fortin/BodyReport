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
        ITrainingExercisesService _trainingExercisesService = null;
        IUserInfosService _userInfosService = null;

        public TrainingDayManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingDayModule = new TrainingDayModule(DbContext);

            _trainingExercisesService = WebAppConfiguration.ServiceProvider.GetService<ITrainingExercisesService>();
            _userInfosService = WebAppConfiguration.ServiceProvider.GetService<IUserInfosService>();
            ((BodyReportService)_trainingExercisesService).SetDbContext(DbContext); // for use same transaction
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
            var trainingDayList = FindTrainingDay(AppUtils.GetUserUnit(_userInfosService, trainingDay.UserId), trainingDayCriteria, trainingDayScenario);
            int trainingDayId = 1;
            if (trainingDayList != null && trainingDayList.Count > 0)
            {
                trainingDayId = trainingDayList.Max(td => td.TrainingDayId) + 1;
            }

            trainingDay.TrainingDayId = trainingDayId;
            // no need transaction, only header

            TrainingDay trainingDayResult = null;
            trainingDayResult = _trainingDayModule.Create(trainingDay, AppUtils.GetUserUnit(_userInfosService, trainingDay.UserId));
            SynchroManager.TrainingDayChange(DbContext, trainingDayResult);

            if (trainingDay.TrainingExercises != null)
            {
                trainingDayResult.TrainingExercises = new List<TrainingExercise>();
                foreach (var trainingExercise in trainingDay.TrainingExercises)
                {
                    trainingDayResult.TrainingExercises.Add(_trainingExercisesService.CreateTrainingExercise(trainingExercise));
                }
            }

            return trainingDayResult;
        }

        private TrainingExerciseCriteria CreateTrainingExerciseCriteria(TrainingDay trainingDay)
        {
            return new TrainingExerciseCriteria()
            {
                UserId = new StringCriteria() { Equal = trainingDay.UserId },
                Year = new IntegerCriteria() { Equal = trainingDay.Year },
                WeekOfYear = new IntegerCriteria() { Equal = trainingDay.WeekOfYear },
                DayOfWeek = new IntegerCriteria() { Equal = trainingDay.DayOfWeek },
                TrainingDayId = new IntegerCriteria() { Equal = trainingDay.TrainingDayId }
            };
        }

        internal TrainingDay GetTrainingDay(TrainingDayKey key, TrainingDayScenario trainingDayScenario)
        {
            var trainingDay = _trainingDayModule.Get(key, AppUtils.GetUserUnit(_userInfosService, key.UserId));

            if (trainingDayScenario.ManageExercise && trainingDay != null)
            {
                var trainingExerciseCriteria = CreateTrainingExerciseCriteria(trainingDay);
                trainingDay.TrainingExercises = _trainingExercisesService.FindTrainingExercise(trainingExerciseCriteria);
            }

            return trainingDay;
        }

        internal List<TrainingDay> FindTrainingDay(TUnitType userUnit, TrainingDayCriteria trainingDayCriteria, TrainingDayScenario trainingDayScenario)
        {
            var trainingDays = _trainingDayModule.Find(userUnit, trainingDayCriteria);

            if (trainingDayScenario != null && trainingDayScenario.ManageExercise && trainingDays != null && trainingDays.Count > 0)
            {
                var criteriaList = new CriteriaList<TrainingExerciseCriteria>();
                string userId = null;
                foreach (var trainingDay in trainingDays)
                {
                    criteriaList.Add(CreateTrainingExerciseCriteria(trainingDay));
                    if(userId == null)
                    {
                        userId = trainingDay.UserId;
                    }
                }
                var trainingExerciseList = _trainingExercisesService.FindTrainingExercise(criteriaList);
                if (trainingExerciseList != null)
                {
                    foreach (var trainingDay in trainingDays)
                    {
                        foreach (var trainingExercise in trainingExerciseList)
                        {
                            if (trainingExercise != null &&
                               (trainingDay.UserId != null && trainingDay.UserId == trainingExercise.UserId) &&
                               (trainingDay.Year == trainingExercise.Year) &&
                               (trainingDay.WeekOfYear == trainingExercise.WeekOfYear) &&
                               (trainingDay.DayOfWeek == trainingExercise.DayOfWeek) &&
                               (trainingDay.TrainingDayId == trainingExercise.TrainingDayId))
                            {
                                if (trainingDay.TrainingExercises == null)
                                    trainingDay.TrainingExercises = new List<TrainingExercise>();
                                trainingDay.TrainingExercises.Add(trainingExercise);
                            }
                        }
                    }
                }
            }

            return trainingDays;
        }

        internal TrainingDay UpdateTrainingDay(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario)
        {
            TrainingDay trainingDayResult = null;
            
            trainingDayResult = _trainingDayModule.Update(trainingDay, AppUtils.GetUserUnit(_userInfosService, trainingDay.UserId));
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
                var trainingExercisesDb = _trainingExercisesService.FindTrainingExercise(trainingExerciseCriteria);
                if (trainingExercisesDb != null && trainingExercisesDb.Count > 0)
                {
                    foreach(var trainingExerciseDb in trainingExercisesDb)
                    {
                        //remove only training exercises who do not present (for keep exercise tempos: retrocompatibility)
                        if (trainingDay.TrainingExercises == null || !trainingDay.TrainingExercises.Any(te => te.Id == trainingExerciseDb.Id))
                            _trainingExercisesService.DeleteTrainingExercise(trainingExerciseDb);
                    }
                }

                if (trainingDay.TrainingExercises != null)
                {
                    trainingDayResult.TrainingExercises = new List<TrainingExercise>();
                    foreach (var trainingExercise in trainingDay.TrainingExercises)
                    {
                        trainingDayResult.TrainingExercises.Add(_trainingExercisesService.UpdateTrainingExercise(trainingExercise, true));
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
                        _trainingExercisesService.DeleteTrainingExercise(trainingExercise);
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
            var userUnit = AppUtils.GetUserUnit(_userInfosService, userId);

            //Search current training day
            var trainingDayCriteria = new TrainingDayCriteria()
            {
                UserId = new StringCriteria() { Equal = userId },
                Year = new IntegerCriteria() { Equal = year },
                WeekOfYear = new IntegerCriteria() { Equal = weekOfYear },
                DayOfWeek = new IntegerCriteria() { Equal = dayOfWeek }
            };
            var currentTrainingDayList = FindTrainingDay(userUnit, trainingDayCriteria, trainingDayScenario);

            trainingDayCriteria.DayOfWeek.Equal = switchDayOfWeek;
            var switchTrainingDayList = FindTrainingDay(userUnit, trainingDayCriteria, trainingDayScenario);

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

        public void ChangeUnitForTrainingExercises(TrainingDay trainingDay, TUnitType oldUnit)
        {
            if(trainingDay != null && trainingDay.Unit != oldUnit && trainingDay.TrainingExercises != null)
            {
                foreach(var trainingExercise in trainingDay.TrainingExercises)
                {
                    if(trainingExercise.TrainingExerciseSets != null)
                    {
                        foreach(var set in trainingExercise.TrainingExerciseSets)
                        {
                            set.Weight = Utils.TransformWeightToUnitSytem(oldUnit, trainingDay.Unit, set.Weight);
                        }
                    }
                }
            }
        }
    }
}
