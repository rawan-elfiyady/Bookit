using System;
using Bookit.Helpers;
using Bookit.Models;
using Bookit.Repositories;

namespace Bookit.Services;

public class AdminService
{
    private readonly UserRepository _userRepository;
    private readonly BookRepository _bookRepository;
    private readonly BorrowedBooksRepository _borrowedBookRepository;

    public AdminService(UserRepository userRepository, BookRepository bookRepository, BorrowedBooksRepository borrowedBookRepository)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _borrowedBookRepository = borrowedBookRepository;
    }

    // ADD Admin
    public async Task<bool> AddAdmin(CreateUserDto request)
    {
        var existingAdmin = await _userRepository.GetUserByEmail(request.Email);

        if (existingAdmin != null)
        {
            return false;
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var admin = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = hashedPassword,
            Role = "Admin",
            IsApproved = true,
        };

        await _userRepository.SaveUser(admin);

        return true;
    }


    // ADD LIBRARIAN
    public async Task<bool> AddLibrarian(CreateUserDto request)
    {
        var existingLibrarian = await _userRepository.GetUserByEmail(request.Email);

        if (existingLibrarian != null)
        {
            return false;
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var librarian = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = hashedPassword,
            Role = "Librarian",
            IsApproved = true,
        };

        await _userRepository.SaveUser(librarian);

        return true;
    }

    // GET LIBRARIAN 
    public async Task<User?> GetLibrarian(int id)
    {
        var librarian = await _userRepository.GetUserById_Role(id, "Librarian");

        if (librarian != null)
        {
            return librarian;
        }

        return null;
    }

    // UPDATE LIBRARIAN
    public async Task<UpdateDataResponse> UpdateLibrarian(int librarianId, UpdateUserDto request)
    {
        var existingLibrarian = await _userRepository.GetUserById_Role(librarianId, "Librarian");

        if (existingLibrarian == null)
        {
            return new UpdateDataResponse { Success = false, Message = "Librarian Doesn`t Exist" };
        }

        if (!string.IsNullOrEmpty(request.FullName))
        {
            existingLibrarian.Name = request.FullName;
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
        return new UpdateDataResponse { Success = true, Message = "Librarian Updated Successfully" };

    }

    // VIEW PROFILE
    public async Task<User?> ViewProfile(int id)
    {
        return await _userRepository.GetUserById(id);
    }

    // UPDATE PROFILE
    public async Task<UpdateDataResponse> UpdateProfile(int id, UpdateProfileDto request)
    {
        var existingAdmin = await _userRepository.GetUserById_Role(id, "Admin");

        if (existingAdmin == null)
        {
            return new UpdateDataResponse { Success = false, Message = "Admin Doesn`t Exist" };
        }
        if (!string.IsNullOrEmpty(request.Name))
        {
            existingAdmin.Name = request.Name;
        }
        if (!string.IsNullOrEmpty(request.Email))
        {
            existingAdmin.Email = request.Email;
        }
        if (!string.IsNullOrEmpty(request.Password))
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, existingAdmin.Password))
            {
                return new UpdateDataResponse { Success = false, Message = "Wrong Password" };
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            existingAdmin.Password = hashedPassword;
        }

        await _userRepository.UpdateUser(existingAdmin);
        return new UpdateDataResponse { Success = true, Message = "Profile Updated Successfully" };

    }

    // LIST APPROVAL REQUESTS
    public async Task<List<User>> GetPendingLibrariansAsync()
    {
        return await _userRepository.GetPendingLibrarians();
    }

    // LIST LIBRARIANS
    public async Task<List<User>> GetLibrariansAsync()
    {
        return await _userRepository.GetLibrarians();
    }

    // LIST USERS
    public async Task<List<User>> GetUsersAsync()
    {
        return await _userRepository.GetUsers();
    }

    // APPROVE LIBRARIAN REQUEST
    public async Task<bool> ApproveLibrarian(int LibrarianId)
    {
        var Librarian = await _userRepository.GetUserById(LibrarianId);

        if (Librarian != null)
        {
            return await _userRepository.ApproveLibrarian(LibrarianId);
        }

        return false;
    }

    // REJECT LIBRARIAN REQUEST
    public async Task<bool> RejectLibrarian(int LibrarianId)
    {
        var Librarian = await _userRepository.GetUserById(LibrarianId);

        if (Librarian != null)
        {
            return await _userRepository.deleteUser(LibrarianId);
        }
        return false;
    }

    // REMOVE LIBRARIAN
    public async Task<bool> RemoveLibrarian(int LibrarianId)
    {
        var Librarian = await _userRepository.GetUserById(LibrarianId);
        if (Librarian != null)
        {
            return await _userRepository.deleteUser(LibrarianId);
        }

        return false;
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

        await _bookRepository.SaveBook(book);

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

}
