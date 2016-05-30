using BodyReport.Data;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Message;
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
                UserId = new StringCriteria() { Equal = trainingDay.UserId },
                Year = new IntegerCriteria() { Equal = trainingDay.Year },
                WeekOfYear = new IntegerCriteria() { Equal = trainingDay.WeekOfYear },
                DayOfWeek = new IntegerCriteria() { Equal = trainingDay.DayOfWeek },
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

        public TrainingDay UpdateTrainingDay(TrainingDay trainingDay, TrainingDayScenario trainingDayScenario)
        {
            var trainingDayManager = new TrainingDayManager(DbContext);
            return trainingDayManager.UpdateTrainingDay(trainingDay, trainingDayScenario);
        }
    }
}
