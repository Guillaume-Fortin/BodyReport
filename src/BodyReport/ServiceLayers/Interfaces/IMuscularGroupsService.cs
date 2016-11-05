using BodyReport.Message;
using System.Collections.Generic;

namespace BodyReport.ServiceLayers.Interfaces
{
    public interface IMuscularGroupsService
    {
        List<MuscularGroup> FindMuscularGroups();

        MuscularGroup CreateMuscularGroup(MuscularGroup muscularGroup);

        MuscularGroup GetMuscularGroup(MuscularGroupKey key);

        void DeleteMuscularGroup(MuscularGroupKey key);

        MuscularGroup UpdateMuscularGroup(MuscularGroup muscularGroup);

        List<MuscularGroup> UpdateMuscularGroupList(List<MuscularGroup> muscularGroups);
    }
}
