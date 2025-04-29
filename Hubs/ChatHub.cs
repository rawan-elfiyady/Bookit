using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Bookit.Data;
using Bookit.Models;

namespace Bookit.Hubs;

public class ChatHub : Hub
{
    private readonly LibraryDbContext _dbContext;

    public ChatHub(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SendPrivateMessage(string receiverId, string message)
    {
        var senderId = Context.UserIdentifier;

        var sender = await _dbContext.Users.FindAsync(senderId);
        var receiver = await _dbContext.Users.FindAsync(receiverId);

        if (sender == null || receiver == null)
        {
            throw new HubException("Sender or receiver not found.");
        }

        var chatMessage = new ChatMessage
        {
            SenderId = senderId,
            SenderName = sender.Name,
            ReceiverId = receiverId,
            ReceiverName = receiver.Name,
            MessageText = message,
            SentAt = DateTimeOffset.UtcNow
        };

        _dbContext.ChatMessages.Add(chatMessage);
        await _dbContext.SaveChangesAsync();

        await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, sender.Name, message, chatMessage.SentAt);
        await Clients.User(senderId).SendAsync("ReceiveMessage", receiverId, receiver.Name, message, chatMessage.SentAt);
    }
}