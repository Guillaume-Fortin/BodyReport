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
    public class TrainingExerciseManager : ServiceManager
    {
        TrainingExerciseModule _trainingDayExerciseModule = null;
        TrainingExerciseSetModule _trainingDayExerciseSetModule = null;

        public TrainingExerciseManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingDayExerciseModule = new TrainingExerciseModule(_dbContext);
            _trainingDayExerciseSetModule = new TrainingExerciseSetModule(_dbContext);
        }

        public TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise)
        {
            var result = _trainingDayExerciseModule.Create(trainingExercise);
            if(result != null && trainingExercise.TrainingExerciseSets != null)
            {
                result.TrainingExerciseSets = new List<TrainingExerciseSet>();
                foreach (var set in trainingExercise.TrainingExerciseSets)
                {
                    result.TrainingExerciseSets.Add(_trainingDayExerciseSetModule.Create(set));
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
                    var setList = _trainingDayExerciseSetModule.Find(new TrainingExerciseCriteria()
                    {
                        UserId = new StringCriteria() { EqualList = new List<string>() { trainingExercise.UserId } },
                        Year = new IntegerCriteria() { EqualList = new List<int>() { trainingExercise.Year } },
                        WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { trainingExercise.WeekOfYear } },
                        DayOfWeek = new IntegerCriteria() { EqualList = new List<int>() { trainingExercise.DayOfWeek } },
                        TrainingDayId = new IntegerCriteria() { EqualList = new List<int>() { trainingExercise.TrainingDayId } },
                        BodyExerciseId = new IntegerCriteria() { EqualList = new List<int>() { trainingExercise.BodyExerciseId } },
                    });

                    if(setList != null && setList.Count > 0)
                    {
                        foreach (var set in setList)
                        {
                            _trainingDayExerciseSetModule.Delete(set);
                        }
                    }
                }

                result.TrainingExerciseSets = new List<TrainingExerciseSet>();
                foreach (var set in trainingExercise.TrainingExerciseSets)
                {
                    _trainingDayExerciseSetModule.Delete(set);
                    result.TrainingExerciseSets.Add(_trainingDayExerciseSetModule.Update(set));
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
                    UserId = new StringCriteria() { EqualList = new List<string>() { trainingExercise.UserId } },
                    Year = new IntegerCriteria() { EqualList = new List<int>() { trainingExercise.Year } },
                    WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { trainingExercise.WeekOfYear } },
                    DayOfWeek = new IntegerCriteria() { EqualList = new List<int>() { trainingExercise.DayOfWeek } },
                    TrainingDayId = new IntegerCriteria() { EqualList = new List<int>() { trainingExercise.TrainingDayId } },
                    BodyExerciseId = new IntegerCriteria() { EqualList = new List<int>() { trainingExercise.BodyExerciseId } }
                };
                var trainingExerciseSetManager = new TrainingExerciseSetManager(_dbContext);
                trainingExercise.TrainingExerciseSets = trainingExerciseSetManager.FindTrainingExerciseSet(criteria);
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

        public List<TrainingExercise> FindTrainingExercise(CriteriaField criteriaField)
        {
            var trainingExercises = _trainingDayExerciseModule.Find(criteriaField);

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
                var trainingExerciseSetManager = new TrainingExerciseSetManager(_dbContext);
                foreach (var trainingExerciseSet in trainingExercise.TrainingExerciseSets)
                {
                    trainingExerciseSetManager.DeleteTrainingExerciseSet(trainingExerciseSet);
                }
            }
        }
    }
}
