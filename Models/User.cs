using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookit.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string? Name { get; set; }

    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required]
    public string? Password { get; set; }

    [Required]
        [RegularExpression("^(Admin|User|Librarian)$", ErrorMessage = "Invalid role. Allowed values: Admin, User, Librarian.")]
        public string Role { get; set; }

    public bool IsApproved { get; set; } = false;

    public ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();

}
