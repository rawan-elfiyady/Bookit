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
        private readonly EmailService _emailService;


        public LibrarianController(LibrarianServices librarianServices, EmailService emailService)
        {
            _librarianServices = librarianServices;
            _emailService = emailService;
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
        // 1- Get Book By Name
        [HttpGet("book-by-name/{name}")]
        public async Task<IActionResult> FilterBookByName(string name)
        {
            var book = await _librarianServices.GetBookByName(name);

            if (book == null)
            {
                return BadRequest(new { message = "Book Doesn`t Exist" });
            }
            return Ok(book);
        }

        // 2- Get Book By Author
        [HttpGet("book-by-author/{author}")]
        public async Task<IActionResult> FilterBookByAuthor(string author)
        {
            var book = await _librarianServices.GetBooksByAuthor(author);

            if (book == null)
            {
                return BadRequest(new { message = "Author Doesn`t Exist" });
            }
            return Ok(book);
        }
        // 3- Get Book By Category
        [HttpGet("books/{category}")]
        public async Task<IActionResult> FilterBookByCategory(string category)
        {
            var books = await _librarianServices.GetBooksByCategory(category);

            if (books == null)
            {
                return BadRequest(new { message = "category Doesn`t Exist" });
            }
            return Ok(books);
        }
        // 4- Get Available Books
        [HttpGet("available-books")]
        public async Task<IActionResult> GetAvailableBooks()
        {
            var books = await _librarianServices.GetAvailableBooks();

            if (books == null)
            {
                return BadRequest(new { message = "There Is No Available Books" });
            }
            return Ok(books);
        }
        // 5- Get All Books
        [HttpGet("all-books")]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _librarianServices.GetAllBooks();

            if (books == null)
            {
                return BadRequest(new { message = "There Is No Books" });
            }
            return Ok(books);
        }
        // 6- Get Borrowed Books
        [HttpGet("borrowed-books")]
        public async Task<IActionResult> GetBorrowedBooks()
        {
            var books = await _librarianServices.GetBorrowedBooks();

            if (books == null)
            {
                return BadRequest(new { message = "There Is No Borrowed Books" });
            }
            return Ok(books);
        }

        // 7- Get Returned Books
        [HttpGet("returned-books")]
        public async Task<IActionResult> GetReturnedBooks()
        {
            var books = await _librarianServices.GetReturnedBooks();

            if (books == null)
            {
                return BadRequest(new { message = "There Is No Returned Books" });
            }
            return Ok(books);
        }

        // 8- List Borrow Requests
        [HttpGet("borrowing-requests")]
        public async Task<IActionResult> ListBorrowRequests()
        {
            var requests = await _librarianServices.PendingBorrowRequests();

            if (requests == null)
            {
                return BadRequest(new { message = "There Is No Requests" });
            }
            return Ok(requests);
        }

        // 9- List Return Requests
        [HttpGet("Return-requests")]
        public async Task<IActionResult> ListReturnRequests()
        {
            var requests = await _librarianServices.PendingReturnRequests();

            if (requests == null)
            {
                return BadRequest(new { message = "There Is No Requests" });
            }
            return Ok(requests);
        }

        //-------------------------------------------------------------------------------------------------------------------------------

        // POST ENDPOINTS

        // 10- Create Book
        [HttpPost("add-book")]
        public async Task<IActionResult> AddBook([FromBody] CreateBookDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Validation failed.", errors = ModelState });
            }
            var success = await _librarianServices.CreateBook(request);

            if (!success.Success)
            {
                return BadRequest(new { message = success.Message });
            }

            return Ok(new { message = success.Message });
        }

        //---------------------------------------------------------------------------------------------------------------------------------

        // PUT ENDPOINTS

        // 11- Update Book
        [HttpPut("update-book/{id}")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto request)
        {
            var result = await _librarianServices.UpdateBook(id, request);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        // 12- Approve Borrowing Request
        [HttpPut("approve-borrow-request/{bookId}/{userId}")]
        public async Task<IActionResult> ApproveBorrowRequest(int bookId, int userId)
        {
            var user = await _librarianServices.GetUser(userId);
            var success = await _librarianServices.ApproveBorrowBook(bookId, userId);

            if (!success)
            {
                return NotFound(new { message = "Book or user not found" });
            }

            // Send Acceptance email
            await _emailService.SendEmailAsync(
                user.Email,
                "Borrowing Request Accepted",
                $"Dear {user.Name},\n\nWe inform you that your borrowing book request has been accepted,\n\n Enjoy Reading!.\n\nIf you have any questions, contact support."
            );

            return Ok(new { message = "Request Approved successfully" });

        }
        // 13- Approve Return Request
        [HttpPut("approve-return-request/{bookId}/{userId}")]
        public async Task<IActionResult> ApproveReturnRequest(int bookId, int userId)
        {
            var user = await _librarianServices.GetUser(userId);
            var success = await _librarianServices.ApproveReturnBook(bookId, userId);

            if (!success)
            {
                return NotFound(new { message = "Book or user not found" });
            }

            // Send Acceptance email
            await _emailService.SendEmailAsync(
                user.Email,
                "Returning Request Accepted",
                $"Dear {user.Name},\n\nWe inform you that your returning book request has been accepted.\n\nIf you have any questions, contact support."
            );

            return Ok(new { message = "Request Approved successfully" });
        }

        //---------------------------------------------------------------------------------------------------------------------------------

        // DELETE ENDPOINTS

        // 14- Reject Borrow Request
        [HttpDelete("reject-borrow-request/{bookId}/{userId}")]
        public async Task<IActionResult> RejectBorrowRequest(int bookId, int userId, [FromBody] string reason)
        {
            var user = await _librarianServices.GetUser(userId);
            var success = await _librarianServices.RejectBorrowRequest(bookId, userId);

            if (!success)
            {
                return NotFound(new { message = "Book or user not found" });
            }

            // Send Acceptance email
            await _emailService.SendEmailAsync(
                user.Email,
                "Returning Request Accepted",
                $"Dear {user.Name},\n\nWe regret to inform you that your borrowing book request has been rejected \n\nReason: {reason}.\n\nIf you have any questions, contact support."
            );

            return Ok(new { message = "Request rejected successfully" });
        }

        // 15- Remove Book
        [HttpDelete("remove-book/{id}")]
        public async Task<IActionResult> RemoveBook(int id)
        {
            var success = await _librarianServices.RemoveBook(id);
            if (!success)
            {
                return NotFound(new { message = "Book Not Found" });
            }
            return Ok(new { message = "Book removed successfully" });
        }
    }
}
