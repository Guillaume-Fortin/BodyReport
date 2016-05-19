using BodyReport.Crud.Module;
using BodyReport.Crud.Transformer;
using BodyReport.Data;
using BodyReport.Models;
using BodyReport.Resources;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class MuscleManager : ServiceManager
    {
        MuscleModule _muscleModule;
        public MuscleManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _muscleModule = new MuscleModule(_dbContext);
        }

        public List<Muscle> FindMuscles(MuscleCriteria criteria = null)
        {
            return _muscleModule.Find(criteria);
        }

        internal Muscle CreateMuscle(Muscle muscle)
        {
            string name = muscle.Name;
            muscle = _muscleModule.Create(muscle);
            if(muscle != null && muscle.Id > 0)
            {
                //Update Translation Name
                string trKey = MuscleTransformer.GetTranslationKey(muscle.Id);
                Translation.UpdateInDB(trKey, name, _dbContext);
                muscle.Name = Translation.GetInDB(trKey, _dbContext);
            }
            return muscle;
        }

        internal Muscle GetMuscle(MuscleKey key)
        {
            return _muscleModule.Get(key);
        }

        internal void DeleteMuscle(MuscleKey key)
        {
            //Update Translation Name
            Translation.DeleteInDB(MuscleTransformer.GetTranslationKey(key.Id), _dbContext);
            _muscleModule.Delete(key);
        }

        internal Muscle UpdateMuscle(Muscle muscle)
        {
            //Update Translation Name
            Translation.UpdateInDB(MuscleTransformer.GetTranslationKey(muscle.Id), muscle.Name, _dbContext);

            return _muscleModule.Update(muscle);
        }
    }
}
