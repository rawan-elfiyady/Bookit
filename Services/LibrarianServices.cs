using System;
using Bookit.Models;
using Bookit.Repositories;

namespace Bookit.Services;

public class LibrarianServices
{
    private readonly UserRepository _userRepository;
    private readonly BookRepository _bookRepository;
    private readonly BorrowedBooksRepository _borrowedBookRepository;

    public LibrarianServices(UserRepository userRepository, BookRepository bookRepository, BorrowedBooksRepository borrowedBookRepository)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _borrowedBookRepository = borrowedBookRepository;
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

        _userRepository.UpdateUser(existingLibrarian);
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
            Category = request.Image
        };

        _bookRepository.SaveBook(book);

        return new CUBookResponse { Success = false, Message = "Book Already Exist" };
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


        _bookRepository.UpdateBook(existingbook);
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

    // Reject Borrow Request
    public async Task<bool> RejectBorrowRequest(int bookId, int userId)
    {
        var book = await _borrowedBookRepository.GetBookByUserId(bookId, userId);

        if (book != null)
        {
            return await _borrowedBookRepository.DeleteBorrowedBook(bookId, userId);
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
    public async Task<bool> ApproveBorrowBook(int bookId, int userId)
    {
        var book = await _borrowedBookRepository.GetBookByUserId(bookId, userId);

        if (book != null)
        {
            var borrowedBook = await _bookRepository.GetBookById(bookId);
            borrowedBook.Quantity--;
            _bookRepository.UpdateBook(borrowedBook);
            
            return await _borrowedBookRepository.ApproveBorrowRequest(bookId, userId);
        }

        return false;
    }

    // APPROVE Return REQUEST
    public async Task<bool> ApproveReturnBook(int bookId, int userId)
    {
        var book = await _borrowedBookRepository.GetBookByUserId(bookId, userId);

        if (book != null)
        {
            var returnedBook = await _bookRepository.GetBookById(bookId);
            returnedBook.Quantity++;
            _bookRepository.UpdateBook(returnedBook);

            book.ReturnDate = DateTime.Now;
            _borrowedBookRepository.UpdateBook(book);

            return await _borrowedBookRepository.ApproveReturnRequest(bookId, userId);
        }

        return false;
    }

}
