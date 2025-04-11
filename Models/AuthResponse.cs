using System;

namespace Bookit.Models;

public class AuthResponse
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
}
