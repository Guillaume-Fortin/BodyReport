using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public class TrainingJournalTransformer
    {
        public static void ToRow(TrainingJournal bean, TrainingJournalRow row)
        {
            if (bean == null)
                return;

            row.UserId = bean.UserId;
            row.Year = bean.Year;
            row.WeekOfYear = bean.WeekOfYear;
            row.UserHeight = bean.UserHeight;
            row.UserWeight = bean.UserWeight;
        }

        internal static TrainingJournal ToBean(TrainingJournalRow row)
        {
            if (row == null)
                return null;

            var bean = new TrainingJournal();
            bean.UserId = row.UserId;
            bean.Year = row.Year;
            bean.WeekOfYear = row.WeekOfYear;
            bean.UserHeight = row.UserHeight;
            bean.UserWeight = row.UserWeight;
            return bean;
        }
    }
}
