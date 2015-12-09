using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
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
        /// User role
        /// </summary>
        public Role Role { get; set; } = null;
    }
}
