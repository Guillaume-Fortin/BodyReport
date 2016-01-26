using BodyReport.Crud.Module;
using BodyReport.Models;
using Framework;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class TrainingExerciseSetManager : ServiceManager
    {
        TrainingExerciseSetModule _trainingExerciseSetModule = null;

        public TrainingExerciseSetManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingExerciseSetModule = new TrainingExerciseSetModule(_dbContext);
        }

        private TUnitType GetUserUnit(string userId)
        {
            TUnitType unit = TUnitType.Imperial;
            var userInfo = new UserInfoManager(_dbContext).GetUserInfo(new UserInfoKey() { UserId = userId });
            if(userInfo != null)
                unit = userInfo.Unit;
            return unit;
        }

        public TrainingExerciseSet CreateTrainingExerciseSet(TrainingExerciseSet trainingExerciseSet)
        {
            var userUnit = GetUserUnit(trainingExerciseSet.UserId);
            TransformUserUnitToMetricUnit(userUnit, trainingExerciseSet);
            var result = _trainingExerciseSetModule.Create(trainingExerciseSet);
            TransformMetricUnitToUserUnit(userUnit, result);
            return result;
        }

        public TrainingExerciseSet UpdateTrainingExerciseSet(TrainingExerciseSet trainingExerciseSet)
        {
            var userUnit = GetUserUnit(trainingExerciseSet.UserId);
            TransformUserUnitToMetricUnit(userUnit, trainingExerciseSet);
            var result = _trainingExerciseSetModule.Update(trainingExerciseSet);
            TransformMetricUnitToUserUnit(userUnit, result);
            return result;
        }

        public TrainingExerciseSet GetTrainingExerciseSet(TrainingExerciseSetKey key)
        {
            var result = _trainingExerciseSetModule.Get(key);

            if(result != null)
            {
                var userUnit = GetUserUnit(result.UserId);
                TransformMetricUnitToUserUnit(userUnit, result);
            }
            return result;
        }

        public List<TrainingExerciseSet> FindTrainingExerciseSet(CriteriaField criteriaField)
        {
            var resultList = _trainingExerciseSetModule.Find(criteriaField);

            if (resultList != null)
            {
                foreach (var exerciseSet in resultList)
                {
                    var userUnit = GetUserUnit(exerciseSet.UserId);
                    TransformMetricUnitToUserUnit(userUnit, exerciseSet);
                }
            }
            return resultList;
        }

        public void DeleteTrainingExerciseSet(TrainingExerciseSet trainingExerciseSet)
        {
            _trainingExerciseSetModule.Delete(trainingExerciseSet);
        }

        private void TransformUserUnitToMetricUnit(TUnitType userUnit, TrainingExerciseSet trainingExerciseSet)
        {
            if(trainingExerciseSet != null)
                trainingExerciseSet.Weight = Utils.TransformWeightToUnitSytem(userUnit, TUnitType.Metric, trainingExerciseSet.Weight);
        }

        private void TransformMetricUnitToUserUnit(TUnitType userUnit, TrainingExerciseSet trainingExerciseSet)
        {
            if (trainingExerciseSet != null)
                trainingExerciseSet.Weight = Utils.TransformWeightToUnitSytem(TUnitType.Metric, userUnit, trainingExerciseSet.Weight);
        }
    }
}