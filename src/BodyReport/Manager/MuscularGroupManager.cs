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
    public class MuscularGroupManager : BodyReportManager
    {
        MuscularGroupModule _muscularGroupModule;
        public MuscularGroupManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _muscularGroupModule = new MuscularGroupModule(_dbContext);
        }

        private void SaveTranslation(MuscularGroup muscularGroup)
        {
            if (muscularGroup != null)
            {
                string trKey = MuscularGroupTransformer.GetTranslationKey(muscularGroup.Id);
                Translation.UpdateInDB(trKey, muscularGroup.Name, _dbContext);
            }
        }

        private void CompleteTranslation(MuscularGroup muscularGroup)
        {
            if (muscularGroup != null)
            {
                string trKey = MuscularGroupTransformer.GetTranslationKey(muscularGroup.Id);
                muscularGroup.Name = Translation.GetInDB(trKey, _dbContext);
            }
        }

        public List<MuscularGroup> FindMuscularGroups()
        {
            var list = _muscularGroupModule.Find();
            if(list != null)
            {
                foreach (var muscularGroup in list)
                    CompleteTranslation(muscularGroup);
            }
            return list;
        }

        internal MuscularGroup CreateMuscularGroup(MuscularGroup muscularGroup)
        {
            string name = muscularGroup != null ? muscularGroup.Name : null;
            MuscularGroup result = _muscularGroupModule.Create(muscularGroup);
            if (result != null && result.Id != 0)
            {
                result.Name = name;
                SaveTranslation(result);
                CompleteTranslation(result);
            }
            return result;
        }

        internal MuscularGroup GetMuscularGroup(MuscularGroupKey key)
        {
            MuscularGroup result = _muscularGroupModule.Get(key);
            CompleteTranslation(result);
            return result;
        }

        internal void DeleteMuscularGroup(MuscularGroupKey key)
        {
            //Update Translation Name
            Translation.DeleteInDB(MuscularGroupTransformer.GetTranslationKey(key.Id), _dbContext);
            _muscularGroupModule.Delete(key);
        }

        internal MuscularGroup UpdateMuscularGroup(MuscularGroup muscularGroup)
        {
            SaveTranslation(muscularGroup);
            MuscularGroup result = _muscularGroupModule.Update(muscularGroup);
            CompleteTranslation(result);
            return result;
        }
    }
}
