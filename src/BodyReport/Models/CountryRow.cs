using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Models
{
    public class CountryRow
    {
        /// <summary>
        /// Id (Key)
        /// </summary>
        public int Id
        {
            get;
            set;
        }
        
        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Short name
        /// </summary>
        public string ShortName
        {
            get;
            set;
        }
    }
}
