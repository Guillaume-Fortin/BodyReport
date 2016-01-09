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

        public TrainingExerciseManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingDayExerciseModule = new TrainingExerciseModule(_dbContext);
        }

        public TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise)
        {
            return _trainingDayExerciseModule.Create(trainingExercise);
        }

        public TrainingExercise UpdateTrainingExercise(TrainingExercise trainingExercise)
        {
            return _trainingDayExerciseModule.Update(trainingExercise);
        }

        public TrainingExercise GetTrainingExercise(TrainingExerciseKey key)
        {
            return _trainingDayExerciseModule.Get(key);
        }

        public List<TrainingExercise> FindTrainingExercise(CriteriaField criteriaField)
        {
            return _trainingDayExerciseModule.Find(criteriaField);
        }
    }
}
