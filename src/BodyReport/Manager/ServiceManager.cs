using BodyReport.Data;
using BodyReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Manager
{
    public class ServiceManager
    {
        /// <summary>
        /// DataBase context with transaction
        /// </summary>
        protected ApplicationDbContext _dbContext = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">db context</param>
        public ServiceManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
