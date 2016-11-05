using BodyReport.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.ServiceLayers
{
    public class BodyReportService
    {
        /// <summary>
        /// Database db context
        /// </summary>
        protected readonly ApplicationDbContext _dbContext = null;
        public BodyReportService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
