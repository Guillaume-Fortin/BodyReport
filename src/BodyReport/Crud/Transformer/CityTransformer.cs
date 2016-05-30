using BodyReport.Models;
using BodyReport.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Transformer
{
    public static class CityTransformer
    {
        public static void ToRow(City bean, CityRow row)
        {
            if (bean == null)
                return;

            row.CountryId = bean.CountryId;
            row.ZipCode = bean.ZipCode;
            row.Id = bean.Id;
            row.Name = bean.Name;
        }

        internal static City ToBean(CityRow row)
        {
            if (row == null)
                return null;

            var bean = new City();
            bean.CountryId = row.CountryId;
            bean.ZipCode = row.ZipCode;
            bean.Id = row.Id;
            bean.Name = row.Name;
            return bean;
        }
    }
}
