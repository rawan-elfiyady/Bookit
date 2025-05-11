using System;
using Bookit.Models;
using Bookit.Repositories;

namespace Bookit.Service;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly BookRepository _bookRepository;
    private readonly BorrowedBooksRepository _borrowedBookRepository;

    public UserService(UserRepository userRepository, BookRepository bookRepository, BorrowedBooksRepository borrowedBookRepository)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _borrowedBookRepository = borrowedBookRepository;
    }

    // VIEW PROFILE
    public async Task<User?> ViewProfile(int id)
    {
        return await _userRepository.GetUserById(id);
    }

    // UPDATE PROFILE
    public async Task<UpdateDataResponse> UpdateProfile(int id, UpdateProfileDto request)
    {
        var existingUser = await _userRepository.GetUserById_Role(id, "User");

        if (existingUser == null)
        {
            return new UpdateDataResponse { Success = false, Message = "User Doesn`t Exist" };
        }
        if (!string.IsNullOrEmpty(request.Name))
        {
            existingUser.Name = request.Name;
        }
        if (!string.IsNullOrEmpty(request.Email))
        {
            existingUser.Email = request.Email;
        }
        if (!string.IsNullOrEmpty(request.Password) && !string.IsNullOrEmpty(request.NewPassword))
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, existingUser.Password))
            {
                return new UpdateDataResponse { Success = false, Message = "Wrong Password" };
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            existingUser.Password = hashedPassword;
        }

        await _userRepository.UpdateUser(existingUser);
        return new UpdateDataResponse { Success = true, Message = "Profile Updated Successfully" };

    }

    // Get libraians 
    public async Task<List<User>> GetLibrarians()
    {
        return await _userRepository.GetLibrarians();
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------

    // Book Services

    // Get Book By Id
    public async Task<Book?> GetBookByID(int bookId)
    {
        return await _bookRepository.GetBookById(bookId);

    }

    // Get Book By Name
    public async Task<Book?> GetBookByName(string bookName)
    {
        return await _bookRepository.GetBookByName(bookName);

    }

    // Get Books By Author
    public async Task<List<Book>> GetBooksByAuthor(string authorName)
    {
        return await _bookRepository.GetBooksByAuthor(authorName);
    }

    // Get Books By Category
    public async Task<List<Book>> GetBooksByCategory(string categoryName)
    {
        return await _bookRepository.GetBooksByCategory(categoryName);
    }

    // Get Available Books 
    public async Task<List<Book>> GetAvailableBooks()
    {
        return await _bookRepository.GetAvailableBooks();
    }

    // Get All Books
    public async Task<List<Book>> GetAllBooks()
    {
        return await _bookRepository.GetAllBooks();
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------

    // Borrowed Books Services

    // Get Borrowed Books
    public async Task<List<BorrowedBook>> GetBorrowedBooks(int userId)
    {
        return await _borrowedBookRepository.GetUserBorrowedBooks(userId);
    }

    // Request To Borrow Book
    public async Task<RequestResponse> RequestBook(int bookId, int userId)
    {
        var book = await _bookRepository.GetBookById(bookId);
        var user = await _userRepository.GetUserById(userId);

        if (book == null)
        {
            return new RequestResponse { Success = false, Message = "Book Is Not Found" };
        }

        if (book.AvailabilityStatus == false || book.Quantity == 0)
        {
            return new RequestResponse { Success = false, Message = "Book Is Not Available Now" };
        }

        var requestedBook = new BorrowedBook
        {
            UserId = userId,
            BookId = bookId,
            Username = user.Name,
            BookName = book.Name,
            BorrowedDate = DateTime.Now,
            Image = book.Image
        };

        await _borrowedBookRepository.SaveBook(requestedBook);

        return new RequestResponse { Success = true, Message = "Book Is Requested Successfully" };
    }
    // Request To Return Book
    public async Task<RequestResponse> RequestReturn(int borrowedBookId)
    {
        var book = await _borrowedBookRepository.GetBookById(borrowedBookId);

        if (book != null)
        {
            book.IsRequestedToBeReturned = true;
            await _borrowedBookRepository.UpdateBook(book);

            return new RequestResponse { Success = true, Message = "Return Book Request Sent Successfully" };
        }

        return new RequestResponse { Success = false, Message = "Book Is Not Found" };
    }
    // Cancel Request To Return Book
    public async Task<RequestResponse> CancelReturnRequest(int borrowedBookId)
    {
        var book = await _borrowedBookRepository.GetBookById(borrowedBookId);

        if (book != null)
        {
            book.IsRequestedToBeReturned = false;
            await _borrowedBookRepository.UpdateBook(book);

            return new RequestResponse { Success = true, Message = "Return Book Request Canceled Successfully!" };
        }

        return new RequestResponse { Success = false, Message = "Book Is Not Found" };
    }

    // Cancel Borrow Request
    public async Task<RequestResponse> CancelBorrowRequest(int borrowedBookId)
    {
        var book = await _borrowedBookRepository.GetBookById(borrowedBookId);

        if (book != null && !book.IsConfirmed)
        {
            await _borrowedBookRepository.DeleteBorrowedBook(borrowedBookId);
            return new RequestResponse { Success = true, Message = "Borrowing Book Request Canceled Successfully!" };
        }

        return new RequestResponse { Success = false, Message = "Borrowing Book Request Already Accepted." };
    }

    
}
