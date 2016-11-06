using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class UserRoleKey : Key
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Role Id
        /// </summary>
        public string RoleId { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("UserRoleKey_{0}_{1}", UserId, RoleId);
        }
    }

    public class UserRole : UserRoleKey
    {
    }

    public class UserRoleCriteria : CriteriaField
    {
        /// <summary>
        /// User Id
        /// </summary>
        public StringCriteria UserId { get; set; }

        /// <summary>
        /// Role Id
        /// </summary>
        public StringCriteria RoleId { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("UserRoleCriteria_{0}",
                UserId == null ? "null" : UserId.GetCacheKey(),
                RoleId == null ? "null" : RoleId.GetCacheKey());
        }
    }
}
