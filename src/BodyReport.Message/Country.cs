using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class CountryKey : Key
    {
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
            return string.Format("CountryKey_{0}", Id.ToString());
        }
    }

    public class Country : CountryKey
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Short name
        /// </summary>
        public string ShortName
        {
            get;
            set;
        }
    }

    public class CountryCriteria : CriteriaField
    {
        /// <summary>
        /// Id
        /// </summary>
        public IntegerCriteria Id { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("CountryCriteria_{0}",
                Id == null ? "null" : Id.GetCacheKey());
        }
    }
}
