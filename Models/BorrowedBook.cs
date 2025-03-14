using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookit.Models;

public class BorrowedBook
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Required]
    public int BookId { get; set; }

    [ForeignKey("BookId")]
    public Book? Book { get; set; }

    [Required]
    public string? BookName { get; set; }

    [Required]
    public DateTime BorrowedDate { get; set; }

    public DateTime? BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool IsConfirmed { get; set; } = false;
    public bool IsReturned { get; set; } = false;


}
