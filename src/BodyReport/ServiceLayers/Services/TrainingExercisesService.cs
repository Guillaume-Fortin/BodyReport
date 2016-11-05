using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Message;
using BodyReport.ServiceLayers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Services
{
    public class TrainingExercisesService : BodyExercisesService, ITrainingExercisesService
    {
        /// <summary>
        /// Training Exercise Manager
        /// </summary>
        TrainingExerciseManager _trainingExerciseManager = null;
        public TrainingExercisesService(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingExerciseManager = new TrainingExerciseManager(_dbContext);
        }

        public TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise)
        {
            return _trainingExerciseManager.CreateTrainingExercise(trainingExercise);
        }

        public TrainingExercise UpdateTrainingExercise(TrainingExercise trainingExercise, bool manageDeleteLinkItem)
        {
            return _trainingExerciseManager.UpdateTrainingExercise(trainingExercise, manageDeleteLinkItem);
        }

        public TrainingExercise GetTrainingExercise(TrainingExerciseKey key)
        {
            return _trainingExerciseManager.GetTrainingExercise(key);
        }
        public List<TrainingExercise> FindTrainingExercise(TrainingExerciseCriteria trainingExerciseCriteria)
        {
            return _trainingExerciseManager.FindTrainingExercise(trainingExerciseCriteria);
        }

        public void DeleteTrainingExercise(TrainingExercise trainingExercise)
        {
            _trainingExerciseManager.DeleteTrainingExercise(trainingExercise);
        }
    }
}
