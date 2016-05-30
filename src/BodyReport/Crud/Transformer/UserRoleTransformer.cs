using BodyReport.Message;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BodyReport.Crud.Transformer
{
    public class UserRoleTransformer
    {
        public static void ToRow(UserRole bean, IdentityUserRole<string> row)
        {
            if (bean == null)
                return;

            row.UserId = bean.UserId;
            row.RoleId = bean.RoleId;
        }

        internal static UserRole ToBean(IdentityUserRole<string> row)
        {
            if (row == null)
                return null;

            var bean = new UserRole();
            bean.UserId = row.UserId;
            bean.RoleId = row.RoleId;
            return bean;
        }
    }
}
