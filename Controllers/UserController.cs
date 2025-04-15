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

        // 1- Get Book By Name
        [HttpGet("book-by-name/{name}")]
        public async Task<IActionResult> FilterBookByName(string name)
        {
            var book = await _userServices.GetBookByName(name);

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
            var book = await _userServices.GetBooksByAuthor(author);

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
            var books = await _userServices.GetBooksByCategory(category);

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
            var books = await _userServices.GetAvailableBooks();

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
            var books = await _userServices.GetAllBooks();

            if (books == null)
            {
                return BadRequest(new { message = "There Is No Books" });
            }
            return Ok(books);
        }

        // 6- Get Borrowed Books
        [HttpGet("borrowed-books/{id}")]
        public async Task<IActionResult> GetBorrowedBooks(int id)
        {
            var books = await _userServices.GetBorrowedBooks(id);

            if (books == null)
            {
                return BadRequest(new { message = "There Is No Borrowed Books" });
            }
            return Ok(books);
        }


        //-------------------------------------------------------------------------------------------------------------------------------

        // POST ENDPOINTS

        // 7- Request A Book To Borrow
        [HttpPost("request-book/{bookId}/{userId}")]
        public async Task<IActionResult> RequestBook(int bookId, int userId)
        {
            var result = await _userServices.RequestBook(bookId, userId);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        // 7- Send a Return Request
        [HttpPost("request-return-book/{borrowedBookId}")]
        public async Task<IActionResult> RequestReturn(int borrowedBookId)
        {
            var result = await _userServices.RequestReturn(borrowedBookId);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        //----------------------------------------------------------------------------------------------------------------------------------

        // 8- Cancel Borrowing Request
        [HttpDelete("cancel-borrowing-request/{borrowedBookId}")]
        public async Task<IActionResult> CancelBorrowRequest(int borrowedBookId)
        {
            var result = await _userServices.CancelBorrowRequest(borrowedBookId);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        // 9- Cancel Return Request
        [HttpDelete("cancel-return-request/{borrowedBookId}")]
        public async Task<IActionResult> CancelReturnRequest(int borrowedBookId)
        {
            var result = await _userServices.CancelReturnRequest(borrowedBookId);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

    }
}