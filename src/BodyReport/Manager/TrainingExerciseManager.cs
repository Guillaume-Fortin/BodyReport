using BodyReport.Crud.Module;
using BodyReport.Data;
using BodyReport.Models;
using Framework;
using Message;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class TrainingExerciseManager : ServiceManager
    {
        TrainingExerciseModule _trainingDayExerciseModule = null;
        UserInfoModule _userInfoModule = null;
        TrainingExerciseSetManager _trainingExerciseSetManager = null;

        public TrainingExerciseManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingDayExerciseModule = new TrainingExerciseModule(_dbContext);
            _userInfoModule = new UserInfoModule(_dbContext);
            _trainingExerciseSetManager = new TrainingExerciseSetManager(_dbContext);
        }

        private TUnitType GetUserUnit(string userId)
        {
            TUnitType unit = TUnitType.Imperial;
            var userInfo = _userInfoModule.Get(new UserInfoKey() { UserId = userId });
            if (userInfo != null)
                unit = userInfo.Unit;
            return unit;
        }

        public TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise)
        {
            var result = _trainingDayExerciseModule.Create(trainingExercise);
            if (result != null && trainingExercise.TrainingExerciseSets != null)
            {
                TrainingExerciseSet trainingExerciseSet;
                result.TrainingExerciseSets = new List<TrainingExerciseSet>();
                foreach (var set in trainingExercise.TrainingExerciseSets)
                {
                    trainingExerciseSet = _trainingExerciseSetManager.CreateTrainingExerciseSet(set);
                    result.TrainingExerciseSets.Add(trainingExerciseSet);
                }
            }

            return result;
        }

        public TrainingExercise UpdateTrainingExercise(TrainingExercise trainingExercise, bool manageDeleteLinkItem)
        {
            var result = _trainingDayExerciseModule.Update(trainingExercise);
            if (result != null && trainingExercise.TrainingExerciseSets != null)
            {
                if(manageDeleteLinkItem)
                {
                    var setList = _trainingExerciseSetManager.FindTrainingExerciseSet(new TrainingExerciseSetCriteria()
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
                            _trainingExerciseSetManager.DeleteTrainingExerciseSet(set);
                        }
                    }
                }
                
                result.TrainingExerciseSets = new List<TrainingExerciseSet>();
                foreach (var set in trainingExercise.TrainingExerciseSets)
                {
                    result.TrainingExerciseSets.Add(_trainingExerciseSetManager.UpdateTrainingExerciseSet(set));
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
                trainingExercise.TrainingExerciseSets = _trainingExerciseSetManager.FindTrainingExerciseSet(criteria);
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

        public void DeleteTrainingExercise(TrainingExercise trainingExercise)
        {
            _trainingDayExerciseModule.Delete(trainingExercise);

            if (trainingExercise.TrainingExerciseSets != null)
            {
                foreach (var trainingExerciseSet in trainingExercise.TrainingExerciseSets)
                {
                    _trainingExerciseSetManager.DeleteTrainingExerciseSet(trainingExerciseSet);
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
