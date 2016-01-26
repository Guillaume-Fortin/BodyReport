using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public class TranslationTransformer
    {
        public static void ToRow(TranslationVal bean, TranslationRow row)
        {
            if (bean == null)
                return;

            row.CultureId = bean.CultureId;
            row.Key = bean.Key;
            row.Value = bean.Value;
        }

        internal static TranslationVal ToBean(TranslationRow row)
        {
            if (row == null)
                return null;

            var bean = new TranslationVal();
            bean.CultureId = row.CultureId;
            bean.Key = row.Key;
            bean.Value = row.Value;
            return bean;
        }
    }
}
