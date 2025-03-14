using System;
using Bookit.Models;
using Bookit.Repositories;

namespace Bookit.Services;

public class LibrarianServices
{
    private readonly UserRepository _userRepository;

    public LibrarianServices(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // GET USERS
    public async Task<List<User>> GetUsers()
    {
        return await _userRepository.GetUsers();
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

}
