using Message;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public class RoleTransformer
    {
        public static void ToRow(Role bean, IdentityRole row)
        {
            if (bean == null)
                return;

            row.Id = bean.Id;
            row.Name = bean.Name;
            row.NormalizedName = bean.NormalizedName;
        }

        internal static Role ToBean(IdentityRole row)
        {
            if (row == null)
                return null;

            var bean = new Role();
            bean.Id = row.Id;
            bean.Name = row.Name;
            bean.NormalizedName = row.NormalizedName;
            return bean;
        }
    }
}
