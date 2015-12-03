using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message
{
    public class MuscularGroupKey
    {
        /// <summary>
        /// Muscular group Id
        /// </summary>
        public int Id { get; set; } = 0;
    }

    public class MuscularGroup : MuscularGroupKey
    {
        /// <summary>
        /// Muscular group Name
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Image Name
        /// </summary>
        public string ImageName { get; set; }
    }
}
