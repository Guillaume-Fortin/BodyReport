using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface IMusclesService
    {
        List<Muscle> FindMuscles(MuscleCriteria criteria = null);
    }
}
