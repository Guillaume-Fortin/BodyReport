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
    public class TrainingJournalManager : ServiceManager
    {
        TrainingJournalModule _trainingJournalModule = null;
        TrainingJournalDayModule _trainingJournalDayModule = null;
        TrainingJournalDayExerciseModule _trainingJournalDayExerciseModule = null;
        UserInfoModule _userInfoModule = null;

        public TrainingJournalManager(ApplicationDbContext dbContext) : base(dbContext)
        {
            _trainingJournalModule = new TrainingJournalModule(_dbContext);
            _trainingJournalDayModule = new TrainingJournalDayModule(_dbContext);
            _trainingJournalDayExerciseModule = new TrainingJournalDayExerciseModule(_dbContext);
            _userInfoModule = new UserInfoModule(_dbContext);
        }

        internal TrainingJournal GetTrainingJournal(TrainingJournalKey key, bool manageTrainingJournalDay)
        {
            var trainingJournal = _trainingJournalModule.Get(key);

            if (manageTrainingJournalDay && trainingJournal != null)
            {
                var userInfo = _userInfoModule.Get(new UserInfoKey() { UserId = key.UserId });
                if (userInfo != null)
                    trainingJournal.UserUnit = userInfo.Unit;

                var trainingJournalDayCriteria = new TrainingJournalDayCriteria()
                {
                    UserId = new StringCriteria() { EqualList = new List<string>() { key.UserId } },
                    Year = new IntegerCriteria() { EqualList = new List<int>() { key.Year } },
                    WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { key.Year } },
                };
                trainingJournal.TrainingJournalDays = _trainingJournalDayModule.Find(trainingJournalDayCriteria);
                if (trainingJournal.TrainingJournalDays != null)
                {
                    foreach(var trainingJournalDay in trainingJournal.TrainingJournalDays)
                    {
                        var trainingJournalDayExerciseCriteria = new TrainingJournalDayExerciseCriteria()
                        {
                            UserId = new StringCriteria() { EqualList = new List<string>() { trainingJournalDay.UserId } },
                            Year = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.Year } },
                            WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.Year } },
                            DayOfWeek = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.DayOfWeek } },
                            TrainingDayId = new IntegerCriteria() { EqualList = new List<int>() { trainingJournalDay.TrainingDayId } }
                        };
                        trainingJournalDay.TrainingJournalDayExercises = _trainingJournalDayExerciseModule.Find();
                    }
                }
            }

            return trainingJournal;
        }
    }
}
