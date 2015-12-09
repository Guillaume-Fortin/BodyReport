using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public static class UserTransformer
    {
        public static void ToRow(User bean, ApplicationUser row)
        {
            if (bean == null)
                return;

            row.Id = bean.Id;
            row.UserName = bean.Name;
            row.Email = bean.Email;
            row.Suspended = bean.Suspended;
        }

        internal static User ToBean(ApplicationUser row)
        {
            if (row == null)
                return null;

            var bean = new User();
            bean.Id = row.Id;
            bean.Name = row.UserName;
            bean.Email = row.Email;
            bean.Suspended = row.Suspended;
            return bean;
        }
    }
}
