using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookit.Models
{
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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ensure the date is in UTC by default

        // Allow UpdatedAt to be nullable
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] // This can be used if you want the database to auto-generate the UpdatedAt value, otherwise, you can manually set it.
        public DateTime? UpdatedAt { get; set; } = null; // Make nullable

        public ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();

        // Method to convert dates to UTC before saving
        public void ConvertDatesToUtc()
        {
            CreatedAt = CreatedAt.ToUniversalTime();
            if (UpdatedAt.HasValue)
            {
                UpdatedAt = UpdatedAt.Value.ToUniversalTime();
            }
        }
    }
}
