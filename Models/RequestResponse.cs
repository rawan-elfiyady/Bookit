using System;

namespace Bookit.Models;

public class RequestResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
