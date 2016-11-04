using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface IBodyExercisesService
    {
        BodyExercise CreateBodyExercise(BodyExercise bodyExercise);
        BodyExercise GetBodyExercise(BodyExerciseKey key);
        List<BodyExercise> FindBodyExercises(BodyExerciseCriteria criteria = null);
        BodyExercise UpdateBodyExercise(BodyExercise bodyExercise);
        List<BodyExercise> UpdateBodyExerciseList(List<BodyExercise> bodyExercises);
        void DeleteBodyExercise(BodyExerciseKey key);
    }
}
