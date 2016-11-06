using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class RoleKey : Key
    {
        /// <summary>
        /// Role Id
        /// </summary>
        public string Id { get; set; } = string.Empty;

        public override string GetCacheKey()
        {
            return string.Format("RoleKey_{0}", Id.ToString());
        }
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

    public class RoleCriteria : CriteriaField
    {
        /// <summary>
        /// Role Id
        /// </summary>
        public IntegerCriteria Id { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("RoleCriteria_{0}",
                Id == null ? "null" : Id.GetCacheKey());
        }
    }
}
