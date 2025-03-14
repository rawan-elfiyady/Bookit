using System;
using System.ComponentModel.DataAnnotations;

namespace Bookit.Models;

public class RegisterDto
{
    [Required]
    public string Name { get; set; } 

    [Required, EmailAddress]
    public string Email { get; set; } 

    [Required]
    public string Password { get; set; } 

    [Required]
    [RoleValidation(ErrorMessage = "Invalid role. Allowed values: Admin, User, Librarian.")]
    public string Role { get; set; }
}
