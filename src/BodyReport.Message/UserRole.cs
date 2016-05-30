using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class UserRoleKey
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Role Id
        /// </summary>
        public string RoleId { get; set; }
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
    }
}
