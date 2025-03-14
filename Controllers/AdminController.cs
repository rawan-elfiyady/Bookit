using System;
using Bookit.Models;
using Bookit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace Bookit.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly EmailService _emailService;

        public AdminController(AdminService adminService, EmailService emailService)
        {
            _adminService = adminService;
            _emailService = emailService;
        }

        // USER MANAGEMENT

        //----------------------------------------------------------------------------------------------------------------------

        // POST EndPoints

        // 1- Create Librarian
        [HttpPost("add-librarian")]
        public async Task<IActionResult> AddLibrarian([FromBody] CreateUserDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Validation failed.", errors = ModelState });
            }

            var success = await _adminService.AddLibrarian(request);

            if (!success)
            {
                return BadRequest(new { message = "Librarian Already Exist" });
            }

            return Ok(new { message = "Librarian Created Successfully" });

        }

        //---------------------------------------------------------------------------------

        // GET EndPoints

        //2-  View Profile
        [HttpGet("viewProfile/{id}")]
        public async Task<IActionResult> ViewProfile(int id)
        {
            var admin = await _adminService.ViewProfile(id);

            if (admin == null)
            {
                return BadRequest(new { message = "Admin Doesn`t Exist" });
            }

            return Ok(admin);
        }

        // 3-  GET Pending Librarians
        [HttpGet("pending-librarians")]
        public async Task<IActionResult> GetPendingLibrarians()
        {
            var pendingLibrarians = await _adminService.GetPendingLibrariansAsync();

            if (pendingLibrarians.Count() == 0)
            {
                return NotFound(new { message = "No pending librarians found" });
            }
            return Ok(pendingLibrarians);
        }

        // 4-  GET Librarians
        [HttpGet("librarians")]
        public async Task<IActionResult> GetLibrarians()
        {
            var librarians = await _adminService.GetLibrariansAsync();

            if (librarians.Count() == 0)
            {
                return NotFound(new { message = "No pending librarians found" });
            }
            return Ok(librarians);
        }

        // 5-  GET Users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _adminService.GetUsersAsync();

            if (users.Count() == 0)
            {
                return NotFound(new { message = "No pending librarians found" });
            }
            return Ok(users);
        }

        //-----------------------------------------------------------------------------------------

        // PUT EndPoints

        // 6-  Approve Librarian
        [HttpPut("approve-librarian/{id}")]
        public async Task<IActionResult> ApproveLibrarian(int id)
        {
            var success = await _adminService.ApproveLibrarian(id);
            if (!success)
            {
                return NotFound(new { message = "Librarian not found" });
            }

            var librarian = await _adminService.GetLibrarian(id);

            // Send email notification
            string subject = "Account Approved - Bookit Library";
            string body = $"<h3>Congratulations {librarian.Name},</h3><p>Your librarian account has been approved! You can now log in.</p>";
            await _emailService.SendEmailAsync(librarian.Email, subject, body);


            return Ok(new { message = "Librarian approved successfully" });
        }

        // 7-  Update Librarian
        [HttpPut("update-librarian/{id}")]
        public async Task<IActionResult> UpdateLibrarian(int id, UpdateUserDto request)
        {
            var result = await _adminService.UpdateLibrarian(id, request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        // 8-  Update Profile
        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, UpdateProfileDto request)
        {
            var result = await _adminService.UpdateProfile(id, request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        //----------------------------------------------------------------------------------------------------------------
        // DELETE EndPoints

        // 9-  Reject Librarian Request
        [HttpDelete("reject-librarian/{id}")]
        [HttpPost("reject-librarian/{librarianId}")]
        public async Task<IActionResult> RejectLibrarian(int librarianId, [FromBody] string reason)
        {
            var librarian = await _adminService.GetLibrarian(librarianId);

            if (librarian == null || librarian.Role != "Librarian")
            {
                return NotFound(new { message = "Librarian not found." });
            }

            // Send rejection email
            await _emailService.SendEmailAsync(
                librarian.Email,
                "Librarian Registration Rejected",
                $"Dear {librarian.Name},\n\nWe regret to inform you that your registration request has been rejected.\n\nReason: {reason}\n\nIf you have any questions, contact support."
            );

            // Optionally, delete the librarian record
            await _adminService.RejectLibrarian(librarianId);

            return Ok(new { message = "Librarian registration rejected and email sent." });
        }


        // 10 - Remove Librarian
        [HttpDelete("remove-librarian/{id}")]
        public async Task<IActionResult> RemoveLibrarian(int id)
        {
            var success = await _adminService.RemoveLibrarian(id);
            if (!success)
            {
                return NotFound(new { message = "Librarian not found" });
            }

            return Ok(new { message = "Librarian removed successfully" });
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
