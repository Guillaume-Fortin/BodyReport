using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class FieldSort
    {
        public string Name;
        public TFieldSort Sort = TFieldSort.None;
    }

    public abstract class CriteriaField
    {
        /// <summary>
        /// Sorted Fields Name
        /// </summary>
        public List<FieldSort> FieldSortList { get; set; }

        public abstract string GetCacheKey();
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

    public abstract class CriteriaType<T>
    {
        /// <summary>
        /// Equals citeria
        /// </summary>
        public List<T> EqualList { get; set; } = null;

        /// <summary>
        /// NotEquals citeria
        /// </summary>
        public List<T> NotEqualList { get; set; } = null;

        public abstract string GetCacheKey();
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

        public override string GetCacheKey()
        {
            return string.Format("IntegerCriteria_{0}_{1}",
                Equal.HasValue ? Equal.Value.ToString() : "",
                NotEqual.HasValue ? NotEqual.Value.ToString() : "");
        }
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

        public override string GetCacheKey()
        {
            string cacheKey = string.Format("StringCriteria_{0}_{1}_{2}_{3}_{4}_{5}",
                Equal == null ? "null" : Equal,
                NotEqual == null ? "null" : NotEqual,
                IgnoreCase ? "1" : "0",
                GetCacheKeyStringList("StartsWithList_", StartsWithList),
                GetCacheKeyStringList("EndsWithList_", EndsWithList),
                GetCacheKeyStringList("ContainsList_", ContainsList));

            return cacheKey;
        }

        private static string GetCacheKeyStringList(string beginKey, List<string> list)
        {
            string result = beginKey;
            if (list == null)
                result += "null";
            else
            {
                string value;
                for (int i = 0; i < list.Count; i++)
                {
                    value = list[i];
                    result += string.Format("{0}{1}{2}_", i, (char)1, value == null?"null" : value);
                }   
            }
            return result;
        }
    }

    
}
