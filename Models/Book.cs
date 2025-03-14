using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Bookit.Models;

public class Book
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(225)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Author { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public bool AvailabilityStatus { get; set; } = true;

    public string Description { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime UpdatedAt { get; set; }

    public ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();
}
