using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
{
    public class CriteriaField
    {
    }

    public class CriteriaType<T>
    {
        /// <summary>
        /// Equals citeria
        /// </summary>
        public List<T> EqualList { get; set; } = null;

        /// <summary>
        /// Equals citeria
        /// </summary>
        public List<T> NotEqualList { get; set; } = null;
    }

    public class IntegerCriteria : CriteriaType<int>
    {
    }

    public class StringCriteria : CriteriaType<string>
    {
        /// <summary>
        /// Ignore case on String comparaison
        /// </summary>
        public bool IgnoreCase { get; set; }  = true;
        /// <summary>
        /// Equals citeria
        /// </summary>
        public List<string> StartsWithList { get; set; } = null;
        /// <summary>
        /// Equals citeria
        /// </summary>
        public List<string> EndsWithList { get; set; } = null;
        /// <summary>
        /// Equals citeria
        /// </summary>
        public List<string> ContainsList { get; set; } = null;
    }

    
}
