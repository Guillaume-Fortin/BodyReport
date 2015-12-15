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

        public List<BodyExercise> FindBodyExercises()
        {
            return _bodyExerciseModule.Find();
        }

        public BodyExercise GetBodyExercise(BodyExerciseKey key)
        {
            return _bodyExerciseModule.Get(key);
        }

        public List<BodyExercise> FindBodyExercise(CriteriaField criteriaField)
        {
            return _bodyExerciseModule.Find(criteriaField);
        }

        public BodyExercise CreateBodyExercise(BodyExercise bodyExercise)
        {
            //Update Translation Name
            Translation.UpdateInDB(BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id), bodyExercise.Name, _dbContext);

            return _bodyExerciseModule.Create(bodyExercise);
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
