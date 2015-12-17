using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
{
    public class CityKey
    {
        /// <summary>
        /// Id (Key)
        /// </summary>
        public int Id
        {
            get;
            set;
        }
    }

    public class City : CityKey
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
