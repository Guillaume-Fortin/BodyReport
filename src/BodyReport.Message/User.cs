using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class UserKey
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string Id { get; set; } = string.Empty;
    }

    public class User : UserKey
    {
        /// <summary>
        /// User name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// User email
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// User suspended
        /// </summary>
        public bool Suspended { get; set; }
        /// <summary>
        /// RegistrationDate
        /// </summary>
        public virtual DateTime RegistrationDate { get; set; }
        /// <summary>
        /// Last login date
        /// </summary>
        public virtual DateTime LastLoginDate { get; set; }
        /// <summary>
        /// User role
        /// </summary>
        public Role Role { get; set; } = null;
    }

    public class UserCriteria : CriteriaField
    {
        /// <summary>
        /// User Id
        /// </summary>
        public IntegerCriteria Id { get; set; }
        /// <summary>
        /// User Name
        /// </summary>
        public StringCriteria UserName { get; set; }
    }
}
