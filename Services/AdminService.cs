using System;
using Bookit.Helpers;
using Bookit.Models;
using Bookit.Repositories;

namespace Bookit.Services;

public class AdminService
{
    private readonly UserRepository _userRepository;

    public AdminService(UserRepository userRepository)
    {
        _userRepository = userRepository;

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

        _userRepository.SaveUser(librarian);

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

        if (request.IsApproved != null)
        {
            existingLibrarian.IsApproved = request.IsApproved;
        }
        _userRepository.UpdateUser(existingLibrarian);
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

        _userRepository.UpdateUser(existingAdmin);
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




}
