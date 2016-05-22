using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Services
{
    public class TrainingDayService
    {
        ApplicationDbContext _dbContext = null;

        public TrainingDayService(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext;
            }

            set
            {
                _dbContext = value;
            }
        }
        
        public TrainingDay CreateTrainingDay(TrainingDay trainingDay)
        {
            var trainingDayManager = new TrainingDayManager(DbContext);
            
            var trainingDayCriteria = new TrainingDayCriteria()
            {
                UserId = new StringCriteria() { EqualList = new List<string>() { trainingDay.UserId } },
                Year = new IntegerCriteria() { EqualList = new List<int>() { trainingDay.Year } },
                WeekOfYear = new IntegerCriteria() { EqualList = new List<int>() { trainingDay.WeekOfYear } },
                DayOfWeek = new IntegerCriteria() { EqualList = new List<int>() { trainingDay.DayOfWeek } },
            };
            var trainingDayScenario = new TrainingDayScenario()
            {
                ManageExercise = false
            };
            var trainingDayList = trainingDayManager.FindTrainingDay(trainingDayCriteria, trainingDayScenario);
            int trainingDayId = 1;
            if (trainingDayList != null && trainingDayList.Count > 0)
            {
                trainingDayId = trainingDayList.Max(td => td.TrainingDayId) + 1;
            }

            trainingDay.TrainingDayId = trainingDayId;
            // no need transaction, only header
            return trainingDayManager.CreateTrainingDay(trainingDay);
        }

        public TrainingDay UpdateTrainingDay(TrainingDay trainingDay)
        {
            var trainingDayManager = new TrainingDayManager(DbContext);
            trainingDayManager.DeleteTrainingDay(trainingDay);
            return trainingDayManager.UpdateTrainingDay(trainingDay);
        }
    }
}
