using BodyReport.Crud.Module;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class TrainingDayManager : ServiceManager
    {
        TrainingDayModule _trainingDayModule = null;
        TrainingExerciseModule _trainingDayExerciseModule = null;

        public TrainingDayManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingDayModule = new TrainingDayModule(_dbContext);
            _trainingDayExerciseModule = new TrainingExerciseModule(_dbContext);
        }

        internal TrainingDay CreateTrainingDay(TrainingDay trainingJournal)
        {
            return _trainingDayModule.Create(trainingJournal);
        }

        internal TrainingDay GetTrainingDay(TrainingDayKey key, bool manageExercise)
        {
            var trainingDay = _trainingDayModule.Get(key);

            /* if (manageExercise)
             {
                 CompleteTrainingDay(trainingDay);
             }*/

            return trainingDay;
        }

        internal List<TrainingDay> FindTrainingDay(CriteriaField criteriaField, bool manageExercise)
        {
            var trainingDays = _trainingDayModule.Find(criteriaField);

            /* if (manageExercise)
             {
                 CompleteTrainingDay(trainingDay);
             }*/

            return trainingDays;
        }
    }
}
