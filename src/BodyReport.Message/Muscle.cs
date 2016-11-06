using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class MuscleKey : Key
    {
        /// <summary>
        /// Muscle Id
        /// </summary>
        public int Id { get; set; }
        
        public override string GetCacheKey()
        {
            return string.Format("MuscleKey_{0}", Id.ToString());
        }
    }

    /// <summary>
    /// Muscle
    /// </summary>
    public class Muscle : MuscleKey
    {
        /// <summary>
        /// Muscular group Id
        /// </summary>
        public int MuscularGroupId { get; set; }

        /// <summary>
        /// Muscle Name
        /// </summary>
        public string Name { get; set; }
    }

    public class MuscleCriteria : CriteriaField
    {
        /// <summary>
        /// Muscle Id
        /// </summary>
        public IntegerCriteria Id { get; set; }

        /// <summary>
        /// Muscular group Id
        /// </summary>
        public IntegerCriteria MuscularGroupId { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("MuscleCriteria_{0}_{1}",
                Id == null ? "null" : Id.GetCacheKey(),
                MuscularGroupId == null ? "null" : MuscularGroupId.GetCacheKey());
        }
    }
}
