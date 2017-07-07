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
            //convert date to user timezone
            var timeZoneInfo = TimeZoneMapper.GetTimeZoneByOlsonName(userInfo.TimeZoneName);
            if (timeZoneInfo == null)
                timeZoneInfo = TimeZoneInfo.Local;

            var result = new TrainingDayViewModel()
            {
                UserId = trainingDay.UserId,
                Year = trainingDay.Year,
                WeekOfYear = trainingDay.WeekOfYear,
                DayOfWeek = trainingDay.DayOfWeek,
                TrainingDayId = trainingDay.TrainingDayId,
                BeginHour = TimeZoneInfo.ConvertTime(trainingDay.BeginHour, timeZoneInfo),
                EndHour = TimeZoneInfo.ConvertTime(trainingDay.EndHour, timeZoneInfo),
                Unit = trainingDay.Unit
            };

            if (trainingDay.TrainingExercises != null)
            {
                foreach (var trainingExercise in trainingDay.TrainingExercises)
                {
                    if (result.RegroupExerciseUnitType == null)
                        result.RegroupExerciseUnitType = trainingExercise.ExerciseUnitType;
                    else if (result.RegroupExerciseUnitType != trainingExercise.ExerciseUnitType)
                    {
                        result.RegroupExerciseUnitType = null; // Mixt, stop foreach
                        break;
                    }
                }
            }
            

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
                ExerciseUnitType = (int)trainingExercise.ExerciseUnitType,
                RestTime = trainingExercise.RestTime,
                EccentricContractionTempo = trainingExercise.EccentricContractionTempo,
                StretchPositionTempo = trainingExercise.StretchPositionTempo,
                ConcentricContractionTempo = trainingExercise.ConcentricContractionTempo,
                ContractedPositionTempo = trainingExercise.ContractedPositionTempo,
                BodyExerciseName = bodyExercise != null && !string.IsNullOrWhiteSpace(bodyExercise.Name) ? bodyExercise.Name : string.Empty,
                BodyExerciseImage = string.Format("/images/bodyexercises/{0}.png", trainingExercise.BodyExerciseId)
            };

            viewModel.TupleSetReps = new List<Tuple<int, int, double>>();
            if (trainingExercise.TrainingExerciseSets != null)
            {
                foreach (var set in trainingExercise.TrainingExerciseSets)
                {
                    viewModel.TupleSetReps.Add(new Tuple<int, int, double>(
                        set.NumberOfSets,
                        trainingExercise.ExerciseUnitType == TExerciseUnitType.RepetitionNumber ? set.NumberOfReps : set.ExecutionTime,
                        set.Weight));
                }
            }

            return viewModel;
        }
    }
}
