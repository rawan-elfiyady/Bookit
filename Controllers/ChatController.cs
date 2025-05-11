using Microsoft.AspNetCore.Mvc;       // For ApiController, ControllerBase, Route, HttpGet
using System.Linq;                    // For LINQ queries
using System.Threading.Tasks;        // For async/await
using Bookit.Data;                   // For LibraryDbContext
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    private readonly LibraryDbContext _dbContext;

    public ChatController(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("history/{senderId}{receiverId}")]
    public async Task<IActionResult> GetChatHistory(int senderId, int receiverId)
    {
        var messages = await _dbContext.ChatMessages
            .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                        (m.SenderId == receiverId && m.ReceiverId == senderId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return Ok(messages);
    }
}
