using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public class TranslationValKey : Key
    {
        /// <summary>
        /// Culture Id
        /// </summary>
        public int CultureId { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("TranslationValKey_{0}_{1}", CultureId.ToString(), Key);
        }
    }

    public class TranslationVal : TranslationValKey
    {
        public string Value { get; set; }
    }

    public class TranslationValCriteria : CriteriaField
    {
        /// <summary>
        /// Culture Id
        /// </summary>
        public IntegerCriteria CultureId { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public StringCriteria Key { get; set; }

        public override string GetCacheKey()
        {
            return string.Format("TranslationValCriteria_{0}",
                CultureId == null ? "null" : CultureId.GetCacheKey(),
                Key == null ? "null" : Key.GetCacheKey());
        }
    }
}
