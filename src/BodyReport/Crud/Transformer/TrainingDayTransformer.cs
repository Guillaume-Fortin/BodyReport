using BodyReport.Framework;
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
            row.BeginHour = DbUtils.DateToUtc(bean.BeginHour);
            row.EndHour = DbUtils.DateToUtc(bean.EndHour);
            row.ModificationDate = DbUtils.DateToUtc(Utils.DateTimeWithoutMs); // Set modificationDate
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
            bean.BeginHour = DbUtils.DbDateToUtc(row.BeginHour);
            bean.EndHour = DbUtils.DbDateToUtc(row.EndHour);
            bean.ModificationDate = DbUtils.DbDateToUtc(row.ModificationDate);
            return bean;
        }
    }
}
