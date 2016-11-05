using BodyReport.Models;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public static class MuscleTransformer
    {
        public static string GetTranslationKey(int muscleId)
        {
            return string.Format("MUSCLE-{0}", muscleId);
        }

        public static void ToRow(Muscle bean, MuscleRow row)
        {
            if (bean == null)
                return;

            row.Id = bean.Id;
            row.MuscularGroupId = bean.MuscularGroupId;
        }

        internal static Muscle ToBean(MuscleRow row)
        {
            if (row == null)
                return null;

            var bean = new Muscle();
            bean.Id = row.Id;
            bean.MuscularGroupId = row.MuscularGroupId;
            return bean;
        }
    }
}
