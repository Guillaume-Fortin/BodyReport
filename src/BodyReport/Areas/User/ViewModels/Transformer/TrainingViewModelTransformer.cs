using BodyReport.Framework;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.ServiceLayers.Interfaces;

namespace BodyReport.Areas.User.ViewModels.Transformer
{
    public static class TrainingViewModelTransformer
    {
        public static TrainingWeekViewModel TrainingWeekToViewModel(TrainingWeek trainingWeek, IUsersService usersService)
        {
            TrainingWeekViewModel trainingJournalVM = new TrainingWeekViewModel();

            trainingJournalVM.UserId = trainingWeek.UserId;
            trainingJournalVM.Year = trainingWeek.Year;
            trainingJournalVM.WeekOfYear = trainingWeek.WeekOfYear;
            trainingJournalVM.UserHeight = trainingWeek.UserHeight;
            trainingJournalVM.UserWeight = trainingWeek.UserWeight;
            trainingJournalVM.Unit = (int)trainingWeek.Unit;

            var user = usersService.GetUser(new UserKey() { Id = trainingWeek.UserId });
            if (user != null)
                trainingJournalVM.UserName = user.Name;
                
            return trainingJournalVM;
        }

        public static TrainingDayViewModel TrainingDayToViewModel(TrainingDay trainingDay, UserInfo userInfo)
        {
            var result = new TrainingDayViewModel()
            {
                UserId = trainingDay.UserId,
                Year = trainingDay.Year,
                WeekOfYear = trainingDay.WeekOfYear,
                DayOfWeek = trainingDay.DayOfWeek,
                TrainingDayId = trainingDay.TrainingDayId
            };

            //convert date to user timezone
            var timeZoneInfo = TimeZoneMapper.GetTimeZoneByOlsonName(userInfo.TimeZoneName);
            if (timeZoneInfo == null)
                timeZoneInfo = TimeZoneInfo.Local;
            result.BeginHour = TimeZoneInfo.ConvertTime(trainingDay.BeginHour, timeZoneInfo);
            result.EndHour = TimeZoneInfo.ConvertTime(trainingDay.EndHour, timeZoneInfo);

            return result;
        }

        public static TrainingExerciseViewModel TrainingExerciseToViewModel(TrainingExercise trainingExercise, IBodyExercisesService bodyExercisesService)
        {
            var bodyExercise = bodyExercisesService.GetBodyExercise(new BodyExerciseKey() { Id = trainingExercise.BodyExerciseId });

            var viewModel = new TrainingExerciseViewModel()
            {
                UserId = trainingExercise.UserId,
                Year = trainingExercise.Year,
                WeekOfYear = trainingExercise.WeekOfYear,
                DayOfWeek = trainingExercise.DayOfWeek,
                TrainingDayId = trainingExercise.TrainingDayId,
                TrainingExerciseId = trainingExercise.Id,
                BodyExerciseId = trainingExercise.BodyExerciseId,
                RestTime = trainingExercise.RestTime,
                BodyExerciseName = bodyExercise != null && !string.IsNullOrWhiteSpace(bodyExercise.Name) ? bodyExercise.Name : string.Empty,
                BodyExerciseImage = string.Format("/images/bodyexercises/{0}.png", trainingExercise.BodyExerciseId)
            };

            viewModel.TupleSetReps = new List<Tuple<int, int, double>>();
            if (trainingExercise.TrainingExerciseSets != null)
            {
                foreach (var set in trainingExercise.TrainingExerciseSets)
                    viewModel.TupleSetReps.Add(new Tuple<int, int, double>(set.NumberOfSets, set.NumberOfReps, set.Weight));
            }

            return viewModel;
        }
    }
}
