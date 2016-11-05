using BodyReport.Models;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public static class MuscularGroupTransformer
    {
        public static string GetTranslationKey(int muscularGroupId)
        {
            return string.Format("MG-{0}", muscularGroupId);
        }

        public static void ToRow(MuscularGroup bean, MuscularGroupRow row)
        {
            if (bean == null)
                return;

            row.Id = bean.Id;
        }

        internal static MuscularGroup ToBean(MuscularGroupRow row)
        {
            if (row == null)
                return null;

            var bean = new MuscularGroup();
            bean.Id = row.Id;
            return bean;
        }
    }
}
