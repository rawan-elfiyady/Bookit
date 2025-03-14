using System;
using Bookit.Models;
using Bookit.Service;
using Microsoft.AspNetCore.Mvc;

namespace Bookit.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userServices;

        public UserController(UserService userServices)
        {
            _userServices = userServices;
        }

        // USER MANAGEMENT

        //--------------------------------------------------------------------------------------------------------------------------------------

        // GET ENDPOINTS

        // 1-  View Profile
        [HttpGet("viewProfile/{id}")]
        public async Task<IActionResult> ViewProfile(int id)
        {
            var user = await _userServices.ViewProfile(id);

            if (user == null)
            {
                return BadRequest(new { message = "User Doesn`t Exist" });
            }

            return Ok(user);
        }

        // --------------------------------------------------------------------------------------------------

        // PUT ENDPOINTS

        // 2-  Update Profile
        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, UpdateProfileDto request)
        {
            var result = await _userServices.UpdateProfile(id, request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        //--------------------------------------------------------------------------------------------------------------------------

        // BOOK MANAGEMENT

        // GET ENDPOINTS

        //-------------------------------------------------------------------------------------------------------------------------------

        // POST ENDPOINTS

        //---------------------------------------------------------------------------------------------------------------------------------

        // PUT ENDPOINTS

        //---------------------------------------------------------------------------------------------------------------------------------

        // DELETE ENDPOINTS
    }
}