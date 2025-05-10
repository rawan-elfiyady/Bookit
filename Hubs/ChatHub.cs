using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Security.Claims;

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

    public async Task SendPrivateMessage(int senderId, int receiverId, string message)
    {
        // var senderId = Context.UserIdentifier;


        var sender = await _dbContext.Users.FindAsync(senderId);
        var receiver = await _dbContext.Users.FindAsync(receiverId);

        if (sender == null || receiver == null)
        {
            throw new HubException("Sender or receiver not found.");
        }

        // Convert int to string
        string senderIdString = senderId.ToString();
        string receiverIdString = receiverId.ToString();

        var chatMessage = new ChatMessage
        {
            SenderId = senderIdString,
            SenderName = sender.Name,
            ReceiverId = receiverIdString,
            ReceiverName = receiver.Name,
            MessageText = message,
            SentAt = DateTimeOffset.UtcNow
        };

        try
        {
            _dbContext.ChatMessages.Add(chatMessage);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database Error: {ex.Message}");
            throw;
        }

        await Clients.User(receiverIdString).SendAsync("ReceiveMessage", senderId, sender.Name, message, chatMessage.SentAt);
        await Clients.User(senderIdString).SendAsync("ReceiveMessage", receiverId, receiver.Name, message, chatMessage.SentAt);
    }
}