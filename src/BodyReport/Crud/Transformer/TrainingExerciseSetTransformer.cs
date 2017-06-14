using BodyReport.Framework;
using BodyReport.Models;
using BodyReport.Message;

namespace BodyReport.Crud.Transformer
{
    public class TrainingExerciseSetTransformer
    {
        public static void ToRow(TrainingExerciseSet bean, TrainingExerciseSetRow row)
        {
            if (bean == null)
                return;

            row.UserId = bean.UserId;
            row.Year = bean.Year;
            row.WeekOfYear = bean.WeekOfYear;
            row.DayOfWeek = bean.DayOfWeek;
            row.TrainingDayId = bean.TrainingDayId;
            row.TrainingExerciseId = bean.TrainingExerciseId;
            row.Id = bean.Id;
            row.NumberOfSets = bean.NumberOfSets;
            row.NumberOfReps = bean.NumberOfReps;
            row.Weight = bean.Weight;
            row.Unit = (int)bean.Unit;
            row.ExecutionTime = bean.ExecutionTime;
            row.ModificationDate = DbUtils.DateToUtc(Utils.DateTimeWithoutMs); // Set modificationDate
            if (bean.ObjectVersionNumber > 0) // Retrocompatibility
            {
                row.ExecutionTime = bean.ExecutionTime;
            }
        }

        internal static TrainingExerciseSet ToBean(TrainingExerciseSetRow row)
        {
            if (row == null)
                return null;

            var bean = new TrainingExerciseSet();
            bean.UserId = row.UserId;
            bean.Year = row.Year;
            bean.WeekOfYear = row.WeekOfYear;
            bean.DayOfWeek = row.DayOfWeek;
            bean.TrainingDayId = row.TrainingDayId;
            bean.TrainingExerciseId = row.TrainingExerciseId;
            bean.Id = row.Id;
            bean.NumberOfSets = row.NumberOfSets;
            bean.NumberOfReps = row.NumberOfReps;
            bean.Weight = row.Weight;
            bean.Unit = Utils.IntToEnum<TUnitType>(row.Unit);
            bean.ExecutionTime = row.ExecutionTime.HasValue ? row.ExecutionTime.Value : 0;
            bean.ModificationDate = DbUtils.DbDateToUtc(row.ModificationDate);
            return bean;
        }
    }
}
