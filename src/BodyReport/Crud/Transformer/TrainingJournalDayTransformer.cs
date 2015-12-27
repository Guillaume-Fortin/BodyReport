using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public class TrainingJournalDayTransformer
    {
        public static void ToRow(TrainingJournalDay bean, TrainingJournalDayRow row)
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
        }

        internal static TrainingJournalDay ToBean(TrainingJournalDayRow row)
        {
            if (row == null)
                return null;

            var bean = new TrainingJournalDay();
            bean.UserId = row.UserId;
            bean.Year = row.Year;
            bean.WeekOfYear = row.WeekOfYear;
            bean.DayOfWeek = row.DayOfWeek;
            bean.TrainingDayId = row.TrainingDayId;
            bean.BeginHour = row.BeginHour;
            bean.EndHour = row.EndHour;
            return bean;
        }
    }
}
