using BodyReport.Crud.Module;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    /// <summary>
    /// Manage training journal
    /// </summary>
    public class TrainingWeekManager : ServiceManager
    {
        TrainingWeekModule _trainingWeekModule = null;
        TrainingDayModule _trainingDayModule = null;
        TrainingExerciseModule _trainingDayExerciseModule = null;
        UserInfoModule _userInfoModule = null;

        public TrainingWeekManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingWeekModule = new TrainingWeekModule(_dbContext);
            _trainingDayModule = new TrainingDayModule(_dbContext);
            _trainingDayExerciseModule = new TrainingExerciseModule(_dbContext);
            _userInfoModule = new UserInfoModule(_dbContext);
        }

        internal TrainingWeek CreateTrainingWeek(TrainingWeek trainingJournal)
        {
            return _trainingWeekModule.Create(trainingJournal);
        }

        internal TrainingWeek UpdateTrainingWeek(TrainingWeek trainingWeek)
        {
            return _trainingWeekModule.Update(trainingWeek);
        }

        internal TrainingWeek GetTrainingWeek(TrainingWeekKey key, bool manageTrainingDay)
        {
            var trainingWeek = _trainingWeekModule.Get(key);

            if (manageTrainingDay)
            {
                CompleteTrainingDay(trainingWeek);
            }

            return trainingWeek;
        }

        private void CompleteTrainingDay(TrainingWeek trainingWeek)
        {
            if (trainingWeek != null)
            {
                var userInfo = _userInfoModule.Get(new UserInfoKey() { UserId = trainingWeek.UserId });
                if (userInfo != null)
                    trainingWeek.Unit = userInfo.Unit;

                var trainingDayCriteria = new TrainingDayCriteria()
                {
                    UserId = new StringCriteria() { EqualList = new List<string>() { trainingWeek.UserId } },
                    Year = new IntegerCriteria() { EqualList = new List<int>() { trainingWeek.Year } },
                    WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { trainingWeek.Year } },
                };
                trainingWeek.TrainingDays = _trainingDayModule.Find(trainingDayCriteria);
                if (trainingWeek.TrainingDays != null)
                {
                    foreach (var trainingJournalDay in trainingWeek.TrainingDays)
                    {
                        var trainingExerciseCriteria = new TrainingDayExerciseCriteria()
                        {
                            UserId = new StringCriteria() { EqualList = new List<string>() { trainingJournalDay.UserId } },
                            Year = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.Year } },
                            WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.Year } },
                            DayOfWeek = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.DayOfWeek } },
                            TrainingDayId = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.TrainingDayId } }
                        };
                        trainingJournalDay.TrainingExercises = _trainingDayExerciseModule.Find();
                    }
                }
            }
        }

        public List<TrainingWeek> FindTrainingWeek(CriteriaField criteriaField, bool manageTrainingDay)
        {
            List<TrainingWeek> trainingWeeks = _trainingWeekModule.Find(criteriaField);
            
            if (manageTrainingDay)
            {
                foreach (TrainingWeek trainingJournal in trainingWeeks)
                {
                    CompleteTrainingDay(trainingJournal);
                }
            }

            return trainingWeeks;
        }

        
    }
}
