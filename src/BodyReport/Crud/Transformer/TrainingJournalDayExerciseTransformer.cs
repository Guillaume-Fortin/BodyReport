using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public class TrainingJournalDayExerciseTransformer
    {
        public static void ToRow(TrainingJournalDayExercise bean, TrainingJournalDayExerciseRow row)
        {
            if (bean == null)
                return;

            row.UserId = bean.UserId;
            row.Year = bean.Year;
            row.WeekOfYear = bean.WeekOfYear;
            row.DayOfWeek = bean.DayOfWeek;
            row.TrainingDayId = bean.TrainingDayId;
            row.BodyExerciseId = bean.BodyExerciseId;
            row.NumberOfSets = bean.NumberOfSets;
            row.NumberOfReps = bean.NumberOfReps;
        }

        internal static TrainingJournalDayExercise ToBean(TrainingJournalDayExerciseRow row)
        {
            if (row == null)
                return null;

            var bean = new TrainingJournalDayExercise();
            bean.UserId = row.UserId;
            bean.Year = row.Year;
            bean.WeekOfYear = row.WeekOfYear;
            bean.DayOfWeek = row.DayOfWeek;
            bean.TrainingDayId = row.TrainingDayId;
            bean.BodyExerciseId = row.BodyExerciseId;
            bean.NumberOfSets = row.NumberOfSets;
            bean.NumberOfReps = row.NumberOfReps;
            return bean;
        }
    }
}
