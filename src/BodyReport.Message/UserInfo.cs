using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class UserInfoKey : Key
    {
        /// <summary>
        /// UserId (Key)
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        public override string GetCacheKey()
        {
            return string.Format("UserInfoKey_{0}", UserId);
        }
    }

    public class UserInfo : UserInfoKey
    {
        /// <summary>
        /// Sex
        /// </summary>
        public TSexType Sex
        {
            get;
            set;
        }

        /// <summary>
        /// Unit Type
        /// </summary>
        public TUnitType Unit
        {
            get;
            set;
        }

        /// <summary>
        /// Height
        /// </summary>
        public double Height
        {
            get;
            set;
        }

        /// <summary>
        /// Weight
        /// </summary>
        public double Weight
        {
            get;
            set;
        }

        /// <summary>
        /// ZipCode
        /// </summary>
        public string ZipCode
        {
            get;
            set;
        }

        /// <summary>
        /// CountryId
        /// </summary>
        public int CountryId
        {
            get;
            set;
        }

        /// <summary>
        /// Olson timezone name
        /// </summary>
        public string TimeZoneName
        {
            get;
            set;
        }
    }

    public class UserInfoCriteria : CriteriaField
    {
        /// <summary>
        /// User Id
        /// </summary>
        public StringCriteria UserId { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("UserInfoCriteria_{0}",
                UserId == null ? "null" : UserId.GetCacheKey());
        }
    }
}
