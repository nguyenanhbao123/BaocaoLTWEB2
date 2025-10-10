using Microsoft.AspNetCore.SignalR;

namespace BeverageShop.API.Hubs
{
    public class ChatHub : Hub
    {
        // Join a private chat room
        public async Task JoinRoom(string roomId, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserJoined", userName);
        }

        // Leave a room
        public async Task LeaveRoom(string roomId, string userName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserLeft", userName);
        }

        // Send message to specific room
        public async Task SendMessage(string roomId, string userName, string message, bool isFromAdmin = false)
        {
            var chatMessage = new
            {
                UserName = userName,
                Message = message,
                IsFromAdmin = isFromAdmin,
                SentAt = DateTime.Now,
                ConnectionId = Context.ConnectionId
            };

            await Clients.Group(roomId).SendAsync("ReceiveMessage", chatMessage);
        }

        // Send message to all (admin broadcast)
        public async Task BroadcastMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveBroadcast", new
            {
                Message = message,
                SentAt = DateTime.Now
            });
        }

        // Typing indicator
        public async Task UserTyping(string roomId, string userName)
        {
            await Clients.OthersInGroup(roomId).SendAsync("UserIsTyping", userName);
        }

        // Admin online status
        public async Task AdminOnline()
        {
            await Clients.All.SendAsync("AdminStatusChanged", true);
        }

        public async Task AdminOffline()
        {
            await Clients.All.SendAsync("AdminStatusChanged", false);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
