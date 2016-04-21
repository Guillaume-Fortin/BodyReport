using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Message.WebApi
{
    public class CopyTrainingWeek
    {
        public string UserId { get; set; }

        public int OriginYear { get; set; }

        public int OriginWeekOfYear { get; set; }

        public int Year { get; set; }

        public int WeekOfYear { get; set; }
    }
}
