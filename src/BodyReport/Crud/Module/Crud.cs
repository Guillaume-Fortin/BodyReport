using BodyReport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BodyReport.Crud.Module
{
    public class Crud
    {
        /// <summary>
        /// DataBase context with transaction
        /// </summary>
        protected ApplicationDbContext _dbContext = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContext">db context</param>
        public Crud(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
