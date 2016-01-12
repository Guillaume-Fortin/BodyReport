using BodyReport.Crud.Module;
using BodyReport.Models;
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

        public TrainingExerciseSet CreateTrainingExerciseSet(TrainingExerciseSet trainingExerciseSet)
        {
            return _trainingExerciseSetModule.Create(trainingExerciseSet);
        }

        public TrainingExerciseSet UpdateTrainingExerciseSet(TrainingExerciseSet trainingExerciseSet)
        {
            return _trainingExerciseSetModule.Update(trainingExerciseSet);
        }

        public TrainingExerciseSet GetTrainingExerciseSet(TrainingExerciseSetKey key)
        {
            return _trainingExerciseSetModule.Get(key);
        }

        public List<TrainingExerciseSet> FindTrainingExerciseSet(CriteriaField criteriaField)
        {
            return _trainingExerciseSetModule.Find(criteriaField);
        }

        public void DeleteTrainingExerciseSet(TrainingExerciseSet trainingExerciseSet)
        {
            _trainingExerciseSetModule.Delete(trainingExerciseSet);
        }
    }
}