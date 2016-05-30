using BodyReport.Crud.Module;
using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Models;
using BodyReport.Resources;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    /// <summary>
    /// Manage Muscular groups
    /// </summary>
    public class MuscularGroupManager : ServiceManager
    {
        MuscularGroupModule _muscularGroupModule;
        public MuscularGroupManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _muscularGroupModule = new MuscularGroupModule(_dbContext);
        }

        public List<MuscularGroup> FindMuscularGroups()
        {
            return _muscularGroupModule.Find();
        }

        internal MuscularGroup CreateMuscularGroup(MuscularGroup muscularGroup)
        {
            string name = muscularGroup.Name;
            muscularGroup = _muscularGroupModule.Create(muscularGroup);
            if (muscularGroup != null && muscularGroup.Id > 0)
            {
                //Update Translation Name
                string trKey = MuscularGroupTransformer.GetTranslationKey(muscularGroup.Id);
                Translation.UpdateInDB(trKey, name, _dbContext);
                muscularGroup.Name = Translation.GetInDB(trKey, _dbContext);
            }
            return muscularGroup;
        }

        internal MuscularGroup GetMuscularGroup(MuscularGroupKey key)
        {
            return _muscularGroupModule.Get(key);
        }

        internal void DeleteMuscularGroup(MuscularGroupKey key)
        {
            //Update Translation Name
            Translation.DeleteInDB(MuscularGroupTransformer.GetTranslationKey(key.Id), _dbContext);
            _muscularGroupModule.Delete(key);
        }

        internal MuscularGroup UpdateMuscularGroup(MuscularGroup muscularGroup)
        {
            //Update Translation Name
            Translation.UpdateInDB(MuscularGroupTransformer.GetTranslationKey(muscularGroup.Id), muscularGroup.Name, _dbContext);

            return _muscularGroupModule.Update(muscularGroup);
        }
    }
}
