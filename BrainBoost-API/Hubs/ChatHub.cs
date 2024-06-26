using BrainBoost_API.Models;
using Microsoft.AspNetCore.SignalR;

namespace BrainBoost_API.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinChat(UserConnection conn)
        {
            await Clients.All.SendAsync("recieveMessage", "admin", $"{conn.UserName} has Joined");
        }
        public async Task JoinSpecificChatRoom(UserConnection conn)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom);
            await Clients.Group(conn.ChatRoom).SendAsync("recieveMessage", "admin", $"{conn.UserName} has Joined {conn.ChatRoom}");
        }
    }
}
