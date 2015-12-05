using BodyReport.Crud.Module;
using BodyReport.Crud.Transformer;
using BodyReport.Models;
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
            return _bodyExerciseModule.Create(bodyExercise);
        }

        internal void DeleteBodyExercise(BodyExerciseKey key)
        {
            _bodyExerciseModule.Delete(key);
        }

        internal BodyExercise UpdateBodyExercise(BodyExercise bodyExercise)
        {
            return _bodyExerciseModule.Update(bodyExercise);
        }
    }
}
