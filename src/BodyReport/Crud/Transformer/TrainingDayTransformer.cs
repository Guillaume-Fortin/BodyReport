using BodyReport.Models;
using Framework;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public class TrainingDayTransformer
    {
        public static void ToRow(TrainingDay bean, TrainingDayRow row)
        {
            if (bean == null)
                return;

            row.UserId = bean.UserId;
            row.Year = bean.Year;
            row.WeekOfYear = bean.WeekOfYear;
            row.DayOfWeek = bean.DayOfWeek;
            row.TrainingDayId = bean.TrainingDayId;
            row.BeginHour = bean.BeginHour;
            row.EndHour = bean.EndHour;
            row.ModificationDate = Utils.DateTimeWithoutMs; // Set modificationDate
        }

        internal static TrainingDay ToBean(TrainingDayRow row)
        {
            if (row == null)
                return null;

            var bean = new TrainingDay();
            bean.UserId = row.UserId;
            bean.Year = row.Year;
            bean.WeekOfYear = row.WeekOfYear;
            bean.DayOfWeek = row.DayOfWeek;
            bean.TrainingDayId = row.TrainingDayId;
            bean.BeginHour = row.BeginHour;
            bean.EndHour = row.EndHour;
            bean.ModificationDate = row.ModificationDate;
            return bean;
        }
    }
}
