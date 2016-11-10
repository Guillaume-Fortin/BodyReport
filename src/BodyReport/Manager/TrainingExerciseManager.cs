using BodyReport.Crud.Module;
using BodyReport.Data;
using BodyReport.Models;
using BodyReport.Framework;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.ServiceLayers.Interfaces;
using BodyReport.ServiceLayers;
using Microsoft.Extensions.DependencyInjection;

namespace BodyReport.Manager
{
    public class TrainingExerciseManager : BodyReportManager
    {
        TrainingExerciseModule _trainingDayExerciseModule = null;
        TrainingExerciseSetModule _trainingExerciseSetModule = null;
        IUserInfosService _userInfosService = null;

        public TrainingExerciseManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingDayExerciseModule = new TrainingExerciseModule(_dbContext);
            _trainingExerciseSetModule = new TrainingExerciseSetModule(_dbContext);

            _userInfosService = WebAppConfiguration.ServiceProvider.GetService<IUserInfosService>();
            ((BodyReportService)_userInfosService).SetDbContext(_dbContext); // for use same transaction
        }

        private TUnitType GetUserUnit(string userId)
        {
            TUnitType unit = TUnitType.Imperial;
            var userInfo = _userInfosService.GetUserInfo(new UserInfoKey() { UserId = userId });
            if (userInfo != null)
                unit = userInfo.Unit;
            return unit;
        }

        public TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise)
        {
            var result = _trainingDayExerciseModule.Create(trainingExercise);
            SynchroManager.TrainingExerciseChange(_dbContext, result);

            if (result != null && trainingExercise.TrainingExerciseSets != null)
            {
                TrainingExerciseSet trainingExerciseSet;
                result.TrainingExerciseSets = new List<TrainingExerciseSet>();
                foreach (var set in trainingExercise.TrainingExerciseSets)
                {
                    trainingExerciseSet = _trainingExerciseSetModule.Create(set);
                    result.TrainingExerciseSets.Add(trainingExerciseSet);
                }
            }

            return result;
        }

        public TrainingExercise UpdateTrainingExercise(TrainingExercise trainingExercise, bool manageDeleteLinkItem)
        {
            var result = _trainingDayExerciseModule.Update(trainingExercise);
            SynchroManager.TrainingExerciseChange(_dbContext, result);

            if (result != null && trainingExercise.TrainingExerciseSets != null)
            {
                if(manageDeleteLinkItem)
                {
                    var setList = _trainingExerciseSetModule.Find(new TrainingExerciseSetCriteria()
                    {
                        UserId = new StringCriteria() { Equal = trainingExercise.UserId },
                        Year = new IntegerCriteria() { Equal = trainingExercise.Year },
                        WeekOfYear = new IntegerCriteria() { Equal = trainingExercise.WeekOfYear },
                        DayOfWeek = new IntegerCriteria() { Equal = trainingExercise.DayOfWeek },
                        TrainingDayId = new IntegerCriteria() { Equal = trainingExercise.TrainingDayId },
                        TrainingExerciseId = new IntegerCriteria() { Equal = trainingExercise.Id }
                    });

                    if(setList != null && setList.Count > 0)
                    {
                        foreach (var set in setList)
                        {
                            _trainingExerciseSetModule.Delete(set);
                        }
                    }
                }
                
                result.TrainingExerciseSets = new List<TrainingExerciseSet>();
                foreach (var set in trainingExercise.TrainingExerciseSets)
                {
                    result.TrainingExerciseSets.Add(_trainingExerciseSetModule.Update(set));
                }
            }

            return result;
        }

        private void CompleteTrainingExerciseWithSet(TrainingExercise trainingExercise)
        {
            if (trainingExercise != null)
            {
                var criteria = new TrainingExerciseSetCriteria()
                {
                    UserId = new StringCriteria() { Equal = trainingExercise.UserId },
                    Year = new IntegerCriteria() { Equal = trainingExercise.Year },
                    WeekOfYear = new IntegerCriteria() { Equal = trainingExercise.WeekOfYear },
                    DayOfWeek = new IntegerCriteria() { Equal = trainingExercise.DayOfWeek },
                    TrainingDayId = new IntegerCriteria() { Equal = trainingExercise.TrainingDayId },
                    TrainingExerciseId = new IntegerCriteria() { Equal = trainingExercise.Id }
                };
                trainingExercise.TrainingExerciseSets = _trainingExerciseSetModule.Find(criteria);
            }
        }

        public TrainingExercise GetTrainingExercise(TrainingExerciseKey key)
        {
            var trainingExercise = _trainingDayExerciseModule.Get(key);

            if(trainingExercise != null)
            {
                CompleteTrainingExerciseWithSet(trainingExercise);
            }

            return trainingExercise;
        }

        public List<TrainingExercise> FindTrainingExercise(TrainingExerciseCriteria trainingExerciseCriteria)
        {
            var trainingExercises = _trainingDayExerciseModule.Find(trainingExerciseCriteria);

            if (trainingExercises != null)
            {
                foreach (var trainingExercise in trainingExercises)
                {
                    CompleteTrainingExerciseWithSet(trainingExercise);
                }
            }

            return trainingExercises;
        }

        public void DeleteTrainingExercise(TrainingExerciseKey key)
        {
            var trainingExercise = GetTrainingExercise(key);
            if (trainingExercise != null)
            {
                _trainingDayExerciseModule.Delete(trainingExercise);
                SynchroManager.TrainingExerciseChange(_dbContext, trainingExercise, true);

                if (trainingExercise.TrainingExerciseSets != null)
                {
                    foreach (var trainingExerciseSet in trainingExercise.TrainingExerciseSets)
                    {
                        _trainingExerciseSetModule.Delete(trainingExerciseSet);
                    }
                }
            }
        }

        private void TransformUserUnitToMetricUnit(TUnitType userUnit, TrainingExerciseSet trainingExerciseSet)
        {
            if (trainingExerciseSet != null)
            {
                trainingExerciseSet.Weight = Utils.TransformWeightToUnitSytem(userUnit, TUnitType.Metric, trainingExerciseSet.Weight);
            }
        }

        private void TransformMetricUnitToUserUnit(TUnitType userUnit, TrainingExerciseSet trainingExerciseSet)
        {
            if (trainingExerciseSet != null)
            {
                trainingExerciseSet.Weight = Utils.TransformWeightToUnitSytem(TUnitType.Metric, userUnit, trainingExerciseSet.Weight);
            }
        }
    }
}
