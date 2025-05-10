using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Bookit.Helpers
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            // This gets the user ID from the JWT claim "sub" or "nameidentifier"
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
