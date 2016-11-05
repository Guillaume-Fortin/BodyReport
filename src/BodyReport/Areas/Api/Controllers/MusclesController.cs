using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using BodyReport.Message;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Data;
using BodyReport.Framework;
using Microsoft.AspNetCore.Identity;
using BodyReport.ServiceLayers.Interfaces;
using System;
using BodyReport.Message.Web;

namespace BodyReport.Areas.Api.Controllers
{
    [Area("Api")]
    public class MusclesController : MvcController
    {
        // <summary>
        /// ServiceLayer
        /// </summary>
        IMusclesService _musclesService = null;

        public MusclesController(UserManager<ApplicationUser> userManager,
                                 IMusclesService musclesService) : base(userManager)
        {
            _musclesService = musclesService;
        }

        // Get api/Muscle/Get
        [HttpGet]
        public IActionResult Get(MuscleKey key)
        {
            try
            {
                var result = _musclesService.GetMuscle(key);
                return new OkObjectResult(result); // Muscle
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // Get api/Muscle/Find
        [HttpGet]
        public IActionResult Find(MuscleCriteria criteria)
        {
            try
            {
                var result = _musclesService.FindMuscles(criteria);
                return new OkObjectResult(result); // List<Muscle>
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // POST api/Muscle/UpdateList
        [HttpPost]
		[Authorize(Roles = "Admin")]
        public IActionResult UpdateList([FromBody]List<Muscle> muscles)
        {
            try
            {
                var result = _musclesService.UpdateMuscleList(muscles);
                return new OkObjectResult(result); // Muscle
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // UPDATE api/Muscles/Delete
        [HttpPost]
		[Authorize(Roles = "Admin")]
        public IActionResult Delete([FromBody]MuscleKey key)
        {
            try
            {
                _musclesService.DeleteMuscle(key);
                return new OkObjectResult(true); // bool
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
