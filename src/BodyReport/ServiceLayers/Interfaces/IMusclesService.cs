using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface IMusclesService
    {
        Muscle GetMuscle(MuscleKey key);
        List<Muscle> FindMuscles(MuscleCriteria criteria = null);
        Muscle CreateMuscle(Muscle muscle);
        Muscle UpdateMuscle(Muscle muscle);
        List<Muscle> UpdateMuscleList(List<Muscle> muscles);
        void DeleteMuscle(MuscleKey key);
    }
}
