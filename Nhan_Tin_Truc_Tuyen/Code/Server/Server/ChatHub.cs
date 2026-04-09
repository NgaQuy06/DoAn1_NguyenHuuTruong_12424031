using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace Server
{
    public class ChatHub : Hub // Trung tâm kết nối
    {

        public async Task SendMessage(string user, string message) // Client phải đặt tên hàm như này để gửi cho server
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message); // Gửi cho tất cả Client, ReceiveMessage: Client phải đặt tên hàm như này để nhận dữ liệu
        }

        public async Task InApp(string user)
        {
            await Clients.All.SendAsync("Notification", user + " đã tham gia ứng dụng");
        }

        public async Task OutApp(string user)
        {
            await Clients.All.SendAsync("Notification", user + " đã rời khỏi ứng dụng");
        }
    }
}
