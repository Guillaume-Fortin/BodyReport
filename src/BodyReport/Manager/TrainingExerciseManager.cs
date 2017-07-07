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
        TrainingExerciseModule _trainingExerciseModule = null;
        TrainingExerciseSetModule _trainingExerciseSetModule = null;
        IUserInfosService _userInfosService = null;

        public TrainingExerciseManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingExerciseModule = new TrainingExerciseModule(DbContext);
            _trainingExerciseSetModule = new TrainingExerciseSetModule(DbContext);

            _userInfosService = WebAppConfiguration.ServiceProvider.GetService<IUserInfosService>();
            ((BodyReportService)_userInfosService).SetDbContext(DbContext); // for use same transaction
        }
        
        public TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise)
        {
            var result = _trainingExerciseModule.Create(trainingExercise);
            SynchroManager.TrainingExerciseChange(DbContext, result);

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
            var result = _trainingExerciseModule.Update(trainingExercise);
            SynchroManager.TrainingExerciseChange(DbContext, result);

            if (result != null && trainingExercise.TrainingExerciseSets != null)
            {
                if(manageDeleteLinkItem)
                { // optimisation : massive delete
                    _trainingExerciseSetModule.Delete(new TrainingExerciseSetKey()
                    {
                        UserId = trainingExercise.UserId,
                        Year = trainingExercise.Year,
                        WeekOfYear = trainingExercise.WeekOfYear,
                        DayOfWeek = trainingExercise.DayOfWeek,
                        TrainingDayId = trainingExercise.TrainingDayId,
                        TrainingExerciseId = trainingExercise.Id
                    });

                    result.TrainingExerciseSets = new List<TrainingExerciseSet>();
                    foreach (var set in trainingExercise.TrainingExerciseSets)
                    {
                        result.TrainingExerciseSets.Add(_trainingExerciseSetModule.Create(set));
                    }
                }
                else
                {
                    result.TrainingExerciseSets = new List<TrainingExerciseSet>();
                    foreach (var set in trainingExercise.TrainingExerciseSets)
                    {
                        result.TrainingExerciseSets.Add(_trainingExerciseSetModule.Update(set));
                    }
                }
            }

            return result;
        }

        private TrainingExerciseSetCriteria CreateTrainingExerciseSetCriteria(TrainingExercise trainingExercise)
        {
            return new TrainingExerciseSetCriteria()
            {
                UserId = new StringCriteria() { Equal = trainingExercise.UserId },
                Year = new IntegerCriteria() { Equal = trainingExercise.Year },
                WeekOfYear = new IntegerCriteria() { Equal = trainingExercise.WeekOfYear },
                DayOfWeek = new IntegerCriteria() { Equal = trainingExercise.DayOfWeek },
                TrainingDayId = new IntegerCriteria() { Equal = trainingExercise.TrainingDayId },
                TrainingExerciseId = new IntegerCriteria() { Equal = trainingExercise.Id }
            };
        }

        public TrainingExercise GetTrainingExercise(TrainingExerciseKey key)
        {
            var trainingExercise = _trainingExerciseModule.Get(key);

            if(trainingExercise != null)
            {
                var criteria = CreateTrainingExerciseSetCriteria(trainingExercise);
                trainingExercise.TrainingExerciseSets = _trainingExerciseSetModule.Find(criteria);
            }

            return trainingExercise;
        }

        public List<TrainingExercise> FindTrainingExercise(TrainingExerciseCriteria trainingExerciseCriteria)
        {
            var trainingExercises = _trainingExerciseModule.Find(trainingExerciseCriteria);
            FindTrainingExerciseSet(trainingExercises);
            return trainingExercises;
        }

        public List<TrainingExercise> FindTrainingExercise(CriteriaList<TrainingExerciseCriteria> trainingExerciseCriteriaList)
        {
            var trainingExercises = _trainingExerciseModule.Find(trainingExerciseCriteriaList);
            FindTrainingExerciseSet(trainingExercises);
            return trainingExercises;
        }

        private List<TrainingExercise> FindTrainingExerciseSet(List<TrainingExercise> trainingExercises)
        {
            if (trainingExercises != null && trainingExercises.Count > 0)
            {
                var criteriaList = new CriteriaList<TrainingExerciseSetCriteria>();
                foreach (var trainingExercise in trainingExercises)
                {
                    criteriaList.Add(CreateTrainingExerciseSetCriteria(trainingExercise));
                }
                var trainingExerciseSetList = _trainingExerciseSetModule.Find(criteriaList);
                if (trainingExerciseSetList != null)
                {
                    foreach (var trainingExerciseSet in trainingExerciseSetList)
                    {
                        foreach (var trainingExercise in trainingExercises)
                        {
                            if (trainingExerciseSet != null &&
                               (trainingExercise.UserId != null && trainingExercise.UserId == trainingExerciseSet.UserId) &&
                               (trainingExercise.Year == trainingExerciseSet.Year) &&
                               (trainingExercise.WeekOfYear == trainingExerciseSet.WeekOfYear) &&
                               (trainingExercise.DayOfWeek == trainingExerciseSet.DayOfWeek) &&
                               (trainingExercise.TrainingDayId == trainingExerciseSet.TrainingDayId) &&
                               (trainingExercise.Id == trainingExerciseSet.TrainingExerciseId))
                            {
                                if (trainingExercise.TrainingExerciseSets == null)
                                    trainingExercise.TrainingExerciseSets = new List<TrainingExerciseSet>();
                                trainingExercise.TrainingExerciseSets.Add(trainingExerciseSet);
                            }
                        }
                    }
                }
            }

            return trainingExercises;
        }

        public void DeleteTrainingExercise(TrainingExerciseKey key)
        {
            var trainingExercise = GetTrainingExercise(key);
            if (trainingExercise != null)
            {
                _trainingExerciseModule.Delete(trainingExercise);
                SynchroManager.TrainingExerciseChange(DbContext, trainingExercise, true);

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
