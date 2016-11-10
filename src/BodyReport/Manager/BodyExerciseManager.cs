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
    /// Manage Body Exercises
    /// </summary>
    public class BodyExerciseManager : BodyReportManager
    {
        BodyExerciseModule _bodyExerciseModule = null;

        public BodyExerciseManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _bodyExerciseModule = new BodyExerciseModule(_dbContext);
        }

        private void SaveTranslation(BodyExercise bodyExercise)
        {
            if (bodyExercise != null)
            {
                string trKey = BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id);
                Translation.UpdateInDB(trKey, bodyExercise.Name, _dbContext);
            }
        }

        private void CompleteTranslation(BodyExercise bodyExercise)
        {
            if (bodyExercise != null)
            {
                string trKey = BodyExerciseTransformer.GetTranslationKey(bodyExercise.Id);
                bodyExercise.Name = Translation.GetInDB(trKey, _dbContext);
            }
        }

        public BodyExercise GetBodyExercise(BodyExerciseKey key)
        {
            BodyExercise result = _bodyExerciseModule.Get(key);
            CompleteTranslation(result);
            return result;
        }

        public List<BodyExercise> FindBodyExercises(BodyExerciseCriteria bodyExerciseCriteria = null)
        {
            var list = _bodyExerciseModule.Find(bodyExerciseCriteria);
            if (list != null)
            {
                foreach (var bodyExercise in list)
                    CompleteTranslation(bodyExercise);
            }
            return list;
        }

        public BodyExercise CreateBodyExercise(BodyExercise bodyExercise)
        {
            string name = bodyExercise != null ? bodyExercise.Name : null;
            var result = _bodyExerciseModule.Create(bodyExercise);
            if (result != null && result.Id != 0)
            {
                result.Name = name;
                SaveTranslation(result);
                CompleteTranslation(result);
            }
            return result;
        }

        internal void DeleteBodyExercise(BodyExerciseKey key)
        {
            _bodyExerciseModule.Delete(key);

            //Update Translation Name
            Translation.DeleteInDB(BodyExerciseTransformer.GetTranslationKey(key.Id), _dbContext);
        }

        internal BodyExercise UpdateBodyExercise(BodyExercise bodyExercise)
        {
            SaveTranslation(bodyExercise);

            BodyExercise result = _bodyExerciseModule.Update(bodyExercise);
            CompleteTranslation(result);
            return result;
        }
    }
}
