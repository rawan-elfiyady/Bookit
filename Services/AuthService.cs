using System;
using System.Threading.Tasks;
using Bookit.Helpers;
using Bookit.Models;
using Bookit.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Bookit.Services;

public class AuthService
{
    private readonly UserRepository _userRepository;
    private readonly JwtHelper _jwtHelper;

    public AuthService(UserRepository userRepository, JwtHelper jwtHelper)
    {
        _userRepository = userRepository;
        _jwtHelper = jwtHelper;
    }

    public async Task<AuthResponse> RegisterUser(RegisterDto request)
    {
        var existingUser = await _userRepository.GetUserByEmail(request.Email);
        if (existingUser != null)
        {
            return new AuthResponse { Success = false, Message = "Email already exists" };
        }
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            Password = hashedPassword,
            Role = request.Role,
            IsApproved = request.Role.Equals("Librarian") ? false : true
        };

        await _userRepository.SaveUser(user);

        if (user.Role.Equals("Librarian"))
        {
            return new AuthResponse { Success = true, Message = "Librarian request sent for approval" };
        }

        var token = _jwtHelper.GenerateToken(user);
        return new AuthResponse { Success = true, Token = token, Role = user.Role.ToString() };
    }

    public async Task<AuthResponse> LoginUser(LoginDto request)
    {
        var user = await _userRepository.GetUserByEmail(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return new AuthResponse { Success = false, Message = "Invalid email or password" };
        }

        if (user.Role.Equals("Librarian") && !user.IsApproved)
        {
            return new AuthResponse { Success = false, Message = "Librarian approval pending" };
        }

        var token = _jwtHelper.GenerateToken(user);
        return new AuthResponse { Success = true, Token = token, Role = user.Role.ToString() };
    }


}
