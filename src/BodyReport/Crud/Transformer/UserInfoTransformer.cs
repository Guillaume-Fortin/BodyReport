using BodyReport.Models;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public class UserInfoTransformer
    {
        public static void ToRow(UserInfo bean, UserInfoRow row)
        {
            if (bean == null)
                return;

            row.UserId = bean.UserId;
            row.Height = bean.Height;
            row.Weight = bean.Weight;
            row.Sex = bean.Sex;
            row.ZipCode = bean.ZipCode;
            row.CityId = bean.CityId;
        }

        internal static UserInfo ToBean(UserInfoRow row)
        {
            if (row == null)
                return null;

            var bean = new UserInfo();
            bean.UserId = row.UserId;
            bean.Height = row.Height;
            bean.Weight = row.Weight;
            bean.Sex = row.Sex;
            bean.ZipCode = row.ZipCode;
            bean.CityId = row.CityId;
            return bean;
        }
    }
}
