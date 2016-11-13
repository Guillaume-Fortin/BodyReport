using BodyReport.Data;
using BodyReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class BodyReportManager
    {
        /// <summary>
        /// DataBase context with transaction
        /// </summary>
        private ApplicationDbContext _dbContext = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">db context</param>
        public BodyReportManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext;
            }
        }
    }
}
