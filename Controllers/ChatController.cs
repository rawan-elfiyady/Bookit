using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Bookit.Models;       // Replace with your actual namespace
using Bookit.Data;         // Replace with the namespace of your DbContext

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly LibraryDbContext _db;
    public ChatController(LibraryDbContext db)
    {
        _db = db;
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetChatHistory(string user1Id, string user2Id)
    {
        var messages = await _db.ChatMessages
            .Where(m => 
                (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                (m.SenderId == user2Id && m.ReceiverId == user1Id))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return Ok(messages);
    }
}
