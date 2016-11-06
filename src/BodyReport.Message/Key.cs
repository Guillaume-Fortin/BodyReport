using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Message
{
    public abstract class Key
    {
        public abstract string GetCacheKey();
    }
}
