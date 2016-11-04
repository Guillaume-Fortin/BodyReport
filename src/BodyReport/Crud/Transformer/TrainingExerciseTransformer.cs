using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Message;

namespace BodyReport.Crud.Transformer
{
    public class TrainingExerciseTransformer
    {
        public static void ToRow(TrainingExercise bean, TrainingExerciseRow row)
        {
            if (bean == null)
                return;

            row.UserId = bean.UserId;
            row.Year = bean.Year;
            row.WeekOfYear = bean.WeekOfYear;
            row.DayOfWeek = bean.DayOfWeek;
            row.TrainingDayId = bean.TrainingDayId;
            row.Id = bean.Id;
            row.BodyExerciseId = bean.BodyExerciseId;
            row.RestTime = bean.RestTime;
            row.ModificationDate = DbUtils.DateToUtc(Utils.DateTimeWithoutMs); // Set modificationDate
        }

        internal static TrainingExercise ToBean(TrainingExerciseRow row)
        {
            if (row == null)
                return null;

            var bean = new TrainingExercise();
            bean.UserId = row.UserId;
            bean.Year = row.Year;
            bean.WeekOfYear = row.WeekOfYear;
            bean.DayOfWeek = row.DayOfWeek;
            bean.TrainingDayId = row.TrainingDayId;
            bean.Id = row.Id;
            bean.BodyExerciseId = row.BodyExerciseId;
            bean.RestTime = row.RestTime;
            bean.ModificationDate = DbUtils.DbDateToUtc(row.ModificationDate);
            return bean;
        }
    }
}
