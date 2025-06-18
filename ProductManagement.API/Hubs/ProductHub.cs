using Microsoft.AspNetCore.SignalR;

namespace ProductManagement.API.Hubs
{
    /// <summary>
    /// SignalR Hub for real-time product updates
    /// </summary>
    public class ProductHub : Hub
    {
        /// <summary>
        /// Join a SignalR group for receiving product updates
        /// </summary>
        /// <param name="groupName">Group name (default: ProductUpdates)</param>
        public async Task JoinGroup(string groupName = "ProductUpdates")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        /// <summary>
        /// Leave a SignalR group
        /// </summary>
        /// <param name="groupName">Group name to leave</param>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        // Add connection event handlers for debugging
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client {Context.ConnectionId} connected");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client {Context.ConnectionId} disconnected: {exception?.Message}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}