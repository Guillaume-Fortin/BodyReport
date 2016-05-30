using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class CriteriaField
    {
    }

    public class CriteriaList<T> : List<T> where T : CriteriaField
    {
    }

    public class EqualField<T>
    {
        public T Value = default(T);

        public EqualField(T value)
        {
            Value = value;
        }
    }

    public class CriteriaType<T>
    {
        /// <summary>
        /// Equals citeria
        /// </summary>
        public List<T> EqualList { get; set; } = null;

        /// <summary>
        /// NotEquals citeria
        /// </summary>
        public List<T> NotEqualList { get; set; } = null;
    }

    public class IntegerCriteria : CriteriaType<int>
    {
        /// <summary>
        /// Equal citeria
        /// </summary>
        public int? Equal { get; set; } = null;

        /// <summary>
        /// NotEqual citeria
        /// </summary>
        public int? NotEqual { get; set; } = null;
    }

    public class StringCriteria : CriteriaType<string>
    {
        /// <summary>
        /// Equal citeria
        /// </summary>
        public string Equal { get; set; } = null;

        /// <summary>
        /// NotEqual citeria
        /// </summary>
        public string NotEqual { get; set; } = null;

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
