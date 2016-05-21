using BodyReport.Data;
using BodyReport.Services;
using Message.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace BodyReport.Areas.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Api")]
    public class DatasController : Controller
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        /// <summary>
        /// Hosting Environement
        /// </summary>
        IHostingEnvironment _env = null;

        public DatasController(
            ILoggerFactory loggerFactory,
            IHostingEnvironment env)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<DatasController>();
            _env = env;
        }

        //
        // GET: /Api/Datas/Export
        [HttpGet]
        public IActionResult Export(List<string> tableNameList)
        {
            if (tableNameList == null || tableNameList.Count == 0)
            {
                tableNameList = new List<string>() { "Users", "RoleClaims", "Roles", "UserClaims",
                "UserLogins", "UserRoles", "BodyExercise", "City",
                "Country", "Muscle", "MuscularGroup", "Sequencer", "TrainingDay", "TrainingExercise",
                "TrainingExerciseSet", "TrainingWeek", "Translation", "UserInfo"};
            }
            
            try
            {
                DatabaseService dataService = new DatabaseService(_env, _loggerFactory);
                dataService.ExportDataTables(tableNameList);

                return new OkResult();
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        //
        // GET: /Api/Datas/Import
        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        //
        // POST: /Api/Datas/Import
        [HttpPost]
        public IActionResult Import(IFormFile dataFile, bool insertOnly = false)
        {
            if(dataFile == null)
                return BadRequest();
            try
            {
                DatabaseService dataService = new DatabaseService(_env, _loggerFactory);
                dataService.ImportDataTables(dataFile, insertOnly);

                return new OkResult();
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
