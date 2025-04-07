using System;
using System.ComponentModel.DataAnnotations;

namespace Bookit.Models;

public class UpdateBookDto
{
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Author { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public bool AvailabilityStatus { get; set; } = true;

    public string Description { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

}
