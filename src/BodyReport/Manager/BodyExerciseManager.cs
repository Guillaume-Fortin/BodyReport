using BodyReport.Crud.Module;
using BodyReport.Crud.Transformer;
using BodyReport.Models;
using BodyReport.Resources;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    /// <summary>
    /// Manage Body Exercises
    /// </summary>
    public class BodyExerciseManager : ServiceManager
    {
        BodyExerciseModule _bodyExerciseModule = null;

        public BodyExerciseManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _bodyExerciseModule = new BodyExerciseModule(_dbContext);
        }

        public BodyExercise GetBodyExercise(BodyExerciseKey key)
        {
            return _bodyExerciseModule.Get(key);
        }

        public List<BodyExercise> FindBodyExercises(CriteriaField criteriaField = null)
        {
            return _bodyExerciseModule.Find(criteriaField);
        }

        public BodyExercise CreateBodyExercise(BodyExercise bodyExercise)
        {
            string name = bodyExercise.Name;
            bodyExercise = _bodyExerciseModule.Create(bodyExercise);
            if (bodyExercise != null && bodyExercise.Id > 0)
            {
                //Update Translation Name
                string trKey = BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id);
                Translation.UpdateInDB(trKey, name, _dbContext);
                bodyExercise.Name = Translation.GetInDB(trKey, _dbContext);
            }
            return bodyExercise;
        }

        internal void DeleteBodyExercise(BodyExerciseKey key)
        {
            //Update Translation Name
            Translation.DeleteInDB(MuscularGroupTransformer.GetTranslationKey(key.Id), _dbContext);

            _bodyExerciseModule.Delete(key);
        }

        internal BodyExercise UpdateBodyExercise(BodyExercise bodyExercise)
        {
            //Update Translation Name
            Translation.UpdateInDB(BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id), bodyExercise.Name, _dbContext);

            return _bodyExerciseModule.Update(bodyExercise);
        }
    }
}
