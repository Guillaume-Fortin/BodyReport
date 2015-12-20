using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Models
{
    public class CityRow
    {
        /// <summary>
        /// Id (Key)
        /// </summary>
        public int CountryId
        {
            get;
            set;
        }
        
        /// <summary>
        /// ZipCode (key)
        /// </summary>
        public string ZipCode
        {
            get;
            set;
        }

        /// <summary>
        /// Id (Key)
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// City name
        /// </summary>
        public string Name
        {
            get;
            set;
        }
    }
}
