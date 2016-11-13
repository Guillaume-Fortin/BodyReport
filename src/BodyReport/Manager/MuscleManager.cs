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
    public class MuscleManager : BodyReportManager
    {
        MuscleModule _muscleModule;
        public MuscleManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _muscleModule = new MuscleModule(DbContext);
        }

        private void SaveTranslation(Muscle muscle)
        {
            if (muscle != null)
            {
                string trKey = BodyExerciseTransformer.GetTranslationKey(muscle.Id);
                Translation.UpdateInDB(trKey, muscle.Name, DbContext);
            }
        }

        private void CompleteTranslation(Muscle muscle)
        {
            if (muscle != null)
            {
                string trKey = BodyExerciseTransformer.GetTranslationKey(muscle.Id);
                muscle.Name = Translation.GetInDB(trKey, DbContext);
            }
        }

        public List<Muscle> FindMuscles(MuscleCriteria criteria = null)
        {
            var list = _muscleModule.Find(criteria);
            if (list != null)
            {
                foreach (var muscle in list)
                    CompleteTranslation(muscle);
            }
            return list;
        }

        internal Muscle CreateMuscle(Muscle muscle)
        {
            string name = muscle != null ? muscle.Name : null;
            var result = _muscleModule.Create(muscle);
            if(result != null && result.Id != 0)
            {
                result.Name = name;
                SaveTranslation(result);
                CompleteTranslation(result);
            }
            return result;
        }

        internal Muscle GetMuscle(MuscleKey key)
        {
            var result = _muscleModule.Get(key);
            CompleteTranslation(result);
            return result;
        }

        internal void DeleteMuscle(MuscleKey key)
        {
            //Update Translation Name
            Translation.DeleteInDB(MuscleTransformer.GetTranslationKey(key.Id), DbContext);
            _muscleModule.Delete(key);
        }

        internal Muscle UpdateMuscle(Muscle muscle)
        {
            SaveTranslation(muscle);

            Muscle result = _muscleModule.Update(muscle);
            CompleteTranslation(result);
            return result;
        }
    }
}
