using System;
using Bookit.Models;
using Bookit.Repositories;

namespace Bookit.Services;

public class LibrarianServices
{
    private readonly UserRepository _userRepository;
    private readonly BookRepository _bookRepository;
    private readonly BorrowedBooksRepository _borrowedBookRepository;
    private readonly EmailService _emailService;


    public LibrarianServices(UserRepository userRepository, BookRepository bookRepository, BorrowedBooksRepository borrowedBookRepository, EmailService emailService)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _borrowedBookRepository = borrowedBookRepository;
        _emailService = emailService;

    }

    // GET USERS
    public async Task<List<User>> GetUsers()
    {
        return await _userRepository.GetUsers();
    }

    // Get User
    public async Task<User?> GetUser(int userId)
    {
        return await _userRepository.GetUserById(userId);
    }


    // VIEW PROFILE
    public async Task<User?> ViewProfile(int id)
    {
        return await _userRepository.GetUserById(id);
    }

    // UPDATE PROFILE
    public async Task<UpdateDataResponse> UpdateProfile(int id, UpdateProfileDto request)
    {
        var existingLibrarian = await _userRepository.GetUserById_Role(id, "Librarian");

        if (existingLibrarian == null)
        {
            return new UpdateDataResponse { Success = false, Message = "Librarian Doesn`t Exist" };
        }
        if (!string.IsNullOrEmpty(request.Name))
        {
            existingLibrarian.Name = request.Name;
        }
        if (!string.IsNullOrEmpty(request.Email))
        {
            existingLibrarian.Email = request.Email;
        }
        if (!string.IsNullOrEmpty(request.Password))
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, existingLibrarian.Password))
            {
                return new UpdateDataResponse { Success = false, Message = "Wrong Password" };
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            existingLibrarian.Password = hashedPassword;
        }

        await _userRepository.UpdateUser(existingLibrarian);
        return new UpdateDataResponse { Success = true, Message = "Profile Updated Successfully" };

    }

    //-------------------------------------------------------------------------------------------------------------------------------------------

    // Books Services 

    // Create Book
    public async Task<CUBookResponse> CreateBook(CreateBookDto request)
    {
        var existingBook = await _bookRepository.GetBookByName(request.Name);

        if (existingBook != null)
        {
            return new CUBookResponse { Success = false, Message = "Book Already Exist" };
        }

        var book = new Book
        {
            Name = request.Name,
            Author = request.Author,
            Quantity = request.Quantity,
            AvailabilityStatus = true,
            Description = request.Description,
            Image = request.Image,
            Category = request.Category
        };

        await _bookRepository.SaveBook(book);

        return new CUBookResponse { Success = false, Message = "Book Created Successfully" };
    }

    // Update Book
    public async Task<CUBookResponse> UpdateBook(int bookId, UpdateBookDto request)
    {
        var existingbook = await _bookRepository.GetBookById(bookId);

        if (existingbook == null)
        {
            return new CUBookResponse { Success = false, Message = "Book Doesn`t Exist" };
        }
        if (!string.IsNullOrEmpty(request.Name))
        {
            existingbook.Name = request.Name;
        }
        if (!string.IsNullOrEmpty(request.Author))
        {
            existingbook.Author = request.Author;
        }

        existingbook.Quantity = request.Quantity;

        if (!string.IsNullOrEmpty(request.Description))
        {
            existingbook.Description = request.Description;
        }
        existingbook.AvailabilityStatus = request.AvailabilityStatus;

        if (!string.IsNullOrEmpty(request.Image))
        {
            existingbook.Image = request.Image;
        }
        if (!string.IsNullOrEmpty(request.Category))
        {
            existingbook.Category = request.Category;
        }


        await _bookRepository.UpdateBook(existingbook);
        return new CUBookResponse { Success = true, Message = "Book Updated Successfully" };
    }

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

    // Delete Book
    public async Task<bool> RemoveBook(int bookId)
    {
        var existingBook = await _bookRepository.GetBookById(bookId);

        if (existingBook != null)
        {
            return await _bookRepository.deleteBook(bookId);
        }

        return false;
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------

    // Borrowed Books Services

    // Get Borrowed Books 
    public async Task<List<BorrowedBook>> GetBorrowedBooks()
    {
        return await _borrowedBookRepository.GetBorrowedBooks();
    }

    // Get Returned Books 
    public async Task<List<BorrowedBook>> GetReturnedBooks()
    {
        return await _borrowedBookRepository.GetReturnedBooks();
    }

    // Reject Borrow Request
    public async Task<bool> RejectBorrowRequest(int borrowedBookId)
    {
        var book = await _borrowedBookRepository.GetBookById(borrowedBookId);

        if (book != null)
        {
            return await _borrowedBookRepository.DeleteBorrowedBook(borrowedBookId);
        }
        return false;
    }

    // List Borrow Requests
    public async Task<List<BorrowedBook>> PendingBorrowRequests()
    {
        return await _borrowedBookRepository.GetPendingBorrowRequests();
    }

    // List Return Requests
    public async Task<List<BorrowedBook>> PendingReturnRequests()
    {
        return await _borrowedBookRepository.GetPendingReturnRequests();
    }

    // APPROVE Borrow REQUEST
    public async Task<bool> ApproveBorrowBook(int borrowedBookId, int bookId)
    {
        var book = await _borrowedBookRepository.GetBookById(borrowedBookId);

        if (book != null)
        {
            var borrowedBook = await _bookRepository.GetBookById(bookId);
            borrowedBook.Quantity--;
            if (borrowedBook.Quantity == 0)
            {
                borrowedBook.AvailabilityStatus = false;
            }
            await _bookRepository.UpdateBook(borrowedBook);

            return await _borrowedBookRepository.ApproveBorrowRequest(borrowedBookId);
        }

        return false;
    }

    // APPROVE Return REQUEST
    public async Task<bool> ApproveReturnBook(int borrowedBookId, int bookId)
    {
        var book = await _borrowedBookRepository.GetBookById(borrowedBookId);

        if (book != null)
        {
            var returnedBook = await _bookRepository.GetBookById(bookId);
            if (returnedBook.Quantity == 0)
            {
                returnedBook.AvailabilityStatus = true;
            }
            returnedBook.Quantity++;
            await _bookRepository.UpdateBook(returnedBook);

            book.ReturnDate = DateTime.Now;
            await _borrowedBookRepository.UpdateBook(book);

            return await _borrowedBookRepository.ApproveReturnRequest(borrowedBookId);
        }

        return false;
    }
    
    public async Task SendDueDateAlertsAsync()
    {
        var targetDate = DateTime.UtcNow.Date.AddDays(8); // Alert for 3 days before due

        var borrowedBooks = await _borrowedBookRepository.GetAlertedBook(targetDate);

        foreach (var borrowed in borrowedBooks)
            {
                var email = borrowed.User.Email;
                var subject = "Book Due Reminder";
                var body = $"Dear {borrowed.User.Name},\n\n" +
                        $"Your borrowed book \"{borrowed.Book.Name}\" is due on {borrowed.DueDate:yyyy-MM-dd}.\n" +
                        $"Please return it on time to avoid a penalty.\n\n" +
                        $"Thank you,\nBookit System";

                await _emailService.SendEmailAsync(email, subject, body);
            }
    }


}
