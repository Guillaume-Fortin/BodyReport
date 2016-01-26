using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
{
    public class TranslationValKey
    {
        public int CultureId { get; set; }
        public string Key { get; set; }
    }

    public class TranslationVal : TranslationValKey
    {
        public string Value { get; set; }
    }
}
