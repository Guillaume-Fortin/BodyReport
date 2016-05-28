using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Framework
{
    public static class DbUtils
    {
        /// <summary>
        /// Convert date to db utc date
        /// Warning : db date stored without timestamp
        /// </summary>
        /// <param name="dbDate">database field date</param>
        /// <returns>converted to utc date</returns>
        public static DateTime DateToUtc(DateTime utcDate)
        {
            DateTime date;
            if (utcDate.Kind == DateTimeKind.Utc)
                date = utcDate;
            else
                date = utcDate.ToUniversalTime();
            
            return date;
        }

        /// <summary>
        /// Convert db date without timestamp to universal date
        /// </summary>
        /// <param name="dbDate">database field date</param>
        /// <returns>converted to utc date</returns>
        public static DateTime DbDateToUtc(DateTime dbDate)
        {
            DateTime utcDate;
            if (dbDate.Kind == DateTimeKind.Utc)
                utcDate = dbDate;
            else if (dbDate.Kind == DateTimeKind.Local)
                utcDate = dbDate.ToUniversalTime();
            else
                utcDate = DateTime.SpecifyKind(dbDate, DateTimeKind.Utc);
                
            return utcDate;
        }
    }
}
