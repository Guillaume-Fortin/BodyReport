using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface ITrainingExercisesService
    {
        TrainingExercise CreateTrainingExercise(TrainingExercise trainingExercise);
        TrainingExercise UpdateTrainingExercise(TrainingExercise trainingExercise, bool manageDeleteLinkItem);
        TrainingExercise GetTrainingExercise(TrainingExerciseKey key);
        List<TrainingExercise> FindTrainingExercise(TrainingExerciseCriteria trainingExerciseCriteria);
        void DeleteTrainingExercise(TrainingExercise trainingExercise);
    }
}
