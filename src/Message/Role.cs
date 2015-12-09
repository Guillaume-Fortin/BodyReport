using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
{
    public class RoleKey
    {
        /// <summary>
        /// Role Id
        /// </summary>
        public string Id { get; set; } = string.Empty;
    }

    public class Role : RoleKey
    {
        /// <summary>
        /// Role name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// role Normalized Name
        /// </summary>
        public string NormalizedName { get; set; } = string.Empty;
    }
}
