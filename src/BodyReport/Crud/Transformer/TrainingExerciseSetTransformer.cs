using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            row.BodyExerciseId = bean.BodyExerciseId;
            row.Id = bean.Id;
            row.NumberOfSets = bean.NumberOfSets;
            row.NumberOfReps = bean.NumberOfReps;
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
            bean.BodyExerciseId = row.BodyExerciseId;
            bean.Id = row.Id;
            bean.NumberOfSets = row.NumberOfSets;
            bean.NumberOfReps = row.NumberOfReps;
            return bean;
        }
    }
}
