[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly LibraryDbContext _dbContext;

    public ChatController(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("history/{userId}")]
    public async Task<IActionResult> GetChatHistory(string userId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var messages = await _dbContext.ChatMessages
            .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId) ||
                        (m.SenderId == userId && m.ReceiverId == currentUserId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        return Ok(messages);
    }
}
