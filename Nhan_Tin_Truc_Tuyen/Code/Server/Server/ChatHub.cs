using Microsoft.AspNetCore.SignalR;

namespace Server
{
    public class ChatHub : Hub // Trung tâm kết nối
    {
        public async Task SendMessage(string user, string message) // Client phải đặt tên hàm như này để gửi cho server
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message); // Gửi cho tất cả Client, ReceiveMessage: Client phải đặt tên hàm như này để nhận dữ liệu
        }

        public async Task PingVao(string user)
        {
            await Clients.All.SendAsync("ReceivePing", user);
        }

        public async Task PingRa(string user)
        {
            await Clients.All.SendAsync("ReceivePing", user);
        }
    }
}
