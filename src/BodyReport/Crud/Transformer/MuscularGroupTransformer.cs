using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public static class MuscularGroupTransformer
    {
        public static void ToRow(MuscularGroup bean, MuscularGroupRow row)
        {
            if (bean == null)
                return;

            row.Id = bean.Id;
            row.Name = bean.Name;
        }

        internal static MuscularGroup ToBean(MuscularGroupRow row)
        {
            if (row == null)
                return null;

            var bean = new MuscularGroup();
            bean.Id = row.Id;
            bean.Name = row.Name;
            //Image name is same that name
            bean.ImageName = row.Name + ".png";
            return bean;
        }
    }
}
