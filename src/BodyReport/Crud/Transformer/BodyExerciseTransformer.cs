using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public static class BodyExerciseTransformer
    {
        public static string GetTranslationKey(int bodyExerciseId)
        {
            return string.Format("BE-{0}", bodyExerciseId);
        }

        public static void ToRow(BodyExercise bean, BodyExerciseRow row)
        {
            if (bean == null)
                return;

            row.Id = bean.Id;
            row.MuscleId = bean.MuscleId;
        }

        internal static BodyExercise ToBean(BodyExerciseRow row)
        {
            if (row == null)
                return null;

            var bean = new BodyExercise();
            bean.Id = row.Id;
            bean.Name = Resources.Translation.GetInDB(GetTranslationKey(row.Id));
            //Image name is "{id}.png"
            bean.ImageName = string.Format("{0}.png", row.Id);
            bean.MuscleId = row.MuscleId;

            return bean;
        }
    }
}
