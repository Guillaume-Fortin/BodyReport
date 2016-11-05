using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using BodyReport.Message;
using BodyReport.Manager;
using BodyReport.Models;
using BodyReport.Data;
using BodyReport.Framework;
using BodyReport.ServiceLayers.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using BodyReport.Message.Web;

namespace BodyReport.Areas.Api.Controllers
{    
    [Authorize]
    [Area("Api")]
    public class MuscularGroupsController : MvcController
    {
        // <summary>
        /// ServiceLayer
        /// </summary>
        IMuscularGroupsService _muscularGroupsService;

        public MuscularGroupsController(UserManager<ApplicationUser> userManager,
                                        IMuscularGroupsService muscularGroupsService) : base(userManager)
        {
            _muscularGroupsService = muscularGroupsService;
        }

        // Get api/MuscularGroups/Get
        [HttpGet]
        public IActionResult Get(MuscularGroupKey key)
        {
            try
            {
                var result = _muscularGroupsService.GetMuscularGroup(key);
                return new OkObjectResult(result); // MuscularGroup
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // Get api/MuscularGroups/Find
        [HttpGet]
        public IActionResult Find()
        {
            try
            {
                var result = _muscularGroupsService.FindMuscularGroups();
                return new OkObjectResult(result); // List<MuscularGroup>
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // POST api/MuscularGroups/Update
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update([FromBody]MuscularGroup muscularGroup)
        {
            try
            {
                var result = _muscularGroupsService.UpdateMuscularGroup(muscularGroup);
                return new OkObjectResult(result); // MuscularGroup
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // POST api/MuscularGroups/UpdateList
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateList([FromBody]List<MuscularGroup> muscularGroups)
        {
            try
            {
                var result = _muscularGroupsService.UpdateMuscularGroupList(muscularGroups);
                return new OkObjectResult(result); // List<MuscularGroup>
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }

        // POST api/MuscularGroups/Delete
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete([FromBody]MuscularGroupKey key)
        {
            try
            {
                _muscularGroupsService.DeleteMuscularGroup(key);
                return new OkObjectResult(true); // bool
            }
            catch (Exception exception)
            {
                return BadRequest(new WebApiException("Error", exception));
            }
        }
    }
}
