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

    public async Task SendMessageToUser(int senderId, int receiverId, string messageContent)
    {

        var sender = await _dbContext.Users.FindAsync(senderId);
        var receiver = await _dbContext.Users.FindAsync(receiverId);
        var message = new ChatMessage
        {
            SenderId = senderId,
            SenderName = sender.Name,
            ReceiverId = receiverId,
            ReceiverName = receiver.Name,
            MessageText = messageContent,
        };

        _dbContext.ChatMessages.Add(message);
        await _dbContext.SaveChangesAsync();

        await Clients.Group(receiverId.ToString()).SendAsync("ReceiveMessage", message.Id, senderId, messageContent);
        await Clients.Group(senderId.ToString()).SendAsync("ReceiveMessage", message.Id, senderId, messageContent);

    }
}