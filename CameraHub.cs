using Microsoft.AspNetCore.SignalR;

namespace thermalCamera
{
    public class CameraHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
