using System;
using Bookit.Models;
using Bookit.Repositories;

namespace Bookit.Service;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly BookRepository _bookRepository;

    public UserService(UserRepository userRepository, BookRepository bookRepository)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
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
        if (!string.IsNullOrEmpty(request.Password))
        {
            if (!BCrypt.Net.BCrypt.Verify(request.Password, existingUser.Password))
            {
                return new UpdateDataResponse { Success = false, Message = "Wrong Password" };
            }
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            existingUser.Password = hashedPassword;
        }

        _userRepository.UpdateUser(existingUser);
        return new UpdateDataResponse { Success = true, Message = "Profile Updated Successfully" };

    }

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

    // Get Borrowed Books
    public async Task<List<BorrowedBook>> GetBorrowedBooks(int userId)
    {
        return await _bookRepository.GetUserBorrowedBooks(userId);
    }
}
