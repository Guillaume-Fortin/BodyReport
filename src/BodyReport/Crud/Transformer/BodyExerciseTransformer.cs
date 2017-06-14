using BodyReport.Models;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Framework;

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
            if (bean.ObjectVersionNumber > 0) // Retrocompatibility
            {
                row.ExerciseCategoryType = (int)bean.ExerciseCategoryType;
                row.ExerciseUnitType = (int)bean.ExerciseUnitType;
            }
        }

        internal static BodyExercise ToBean(BodyExerciseRow row)
        {
            if (row == null)
                return null;

            var bean = new BodyExercise();
            bean.Id = row.Id;
            //Image name is "{id}.png"
            bean.ImageName = string.Format("{0}.png", row.Id);
            bean.MuscleId = row.MuscleId;
            bean.ExerciseCategoryType = Utils.IntToEnum<TExerciseCategoryType>(row.ExerciseCategoryType ?? (int)TExerciseCategoryType.Bodybuilding);
            bean.ExerciseUnitType = Utils.IntToEnum<TExerciseUnitType>(row.ExerciseUnitType ?? (int)TExerciseUnitType.RepetitionNumber);

            return bean;
        }
    }
}
