using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class CityKey : Key
    {
        /// <summary>
        /// Country Id (Key)
        /// </summary>
        public int CountryId
        {
            get;
            set;
        }

        /// <summary>
        /// ZipCode (key)
        /// </summary>
        public string ZipCode
        {
            get;
            set;
        }

        /// <summary>
        /// Id (Key)
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        public override string GetCacheKey()
        {
            return string.Format("CityKey_{0}_{1}_{2}", CountryId, ZipCode, Id.ToString());
        }
    }

    public class City : CityKey
    {
        /// <summary>
        /// City name
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }

    public class CityCriteria : CriteriaField
    {
        /// <summary>
        /// Country Id
        /// </summary>
        public IntegerCriteria CountryId { get; set; }

        /// <summary>
        /// ZipCode
        /// </summary>
        public StringCriteria ZipCode { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public IntegerCriteria Id { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("CityCriteria_{0}_{1}_{2}",
                CountryId == null ? "null" : CountryId.GetCacheKey(),
                ZipCode == null ? "null" : ZipCode.GetCacheKey(),
                Id == null ? "null" : Id.GetCacheKey());
        }
    }
}
