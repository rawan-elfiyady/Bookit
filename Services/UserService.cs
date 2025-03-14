using System;
using Bookit.Models;
using Bookit.Repositories;

namespace Bookit.Service;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
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
}
