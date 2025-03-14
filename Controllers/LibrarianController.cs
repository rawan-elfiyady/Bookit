using System;
using Bookit.Models;
using Bookit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace Bookit.Controllers

{
    [Microsoft.AspNetCore.Mvc.Route("librarian")]
    [ApiController]
    public class LibrarianController : ControllerBase
    {
        private readonly LibrarianServices _librarianServices;

        public LibrarianController(LibrarianServices librarianServices)
        {
            _librarianServices = librarianServices;
        }

        // USER MANAGEMENT

        //-------------------------------------------------------------------------------------------------------------------------------------

        // GET ENDPOINTS

        // 1- GET USERS
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _librarianServices.GetUsers();
            if (users == null)
            {
                return BadRequest(new { message = "No Users Found" });
            }

            return Ok(users);
        }

        // 2-  View Profile
        [HttpGet("viewProfile/{id}")]
        public async Task<IActionResult> ViewProfile(int id)
        {
            var librarian = await _librarianServices.ViewProfile(id);

            if (librarian == null)
            {
                return BadRequest(new { message = "Librarian Doesn`t Exist" });
            }

            return Ok(librarian);
        }

        // --------------------------------------------------------------------------------------------------

        // PUT ENDPOINTS

        // 3-  Update Profile
        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, UpdateProfileDto request)
        {
            var result = await _librarianServices.UpdateProfile(id, request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        //--------------------------------------------------------------------------------------------------------------------------

        // BOOK MANAGEMENT
        
        //-------------------------------------------------------------------------------------------------------------------------------------


        // GET ENDPOINTS

        //-------------------------------------------------------------------------------------------------------------------------------

        // POST ENDPOINTS

        //---------------------------------------------------------------------------------------------------------------------------------

        // PUT ENDPOINTS

        //---------------------------------------------------------------------------------------------------------------------------------

        // DELETE ENDPOINTS
    }
}
