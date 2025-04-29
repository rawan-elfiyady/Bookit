using Microsoft.AspNetCore.SignalR;
using Bookit.Models;

namespace Bookit.Hubs;

public class ChatHub : Hub
{
    public async Task JoinChat(UserConnection conn)
    {
        await Clients.All
        .SendAsync("RecieveMessage", "admin", $"{conn.Username} has joined");
    }


    public async Task JoinSpecificChatRoom(UserConnection conn)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom);
        await Clients.Group(conn.ChatRoom)
        .SendAsync("RecieveMessage", "admin", $"{conn.Username} has joined {conn.ChatRoom}");
    }
}