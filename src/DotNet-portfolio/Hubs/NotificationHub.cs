using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DotNet_portfolio.Hubs
{
    /// <summary>
    /// SignalR Hub for handling real-time notifications.
    /// </summary>
    public class NotificationHub : Hub
    {
        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="user">The user sending the message.</param>
        /// <param name="message">The message content.</param>
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
