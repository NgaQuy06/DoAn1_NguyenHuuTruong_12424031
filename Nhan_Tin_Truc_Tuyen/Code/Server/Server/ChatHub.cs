using Microsoft.AspNetCore.SignalR;

namespace Server
{
    public class ChatHub : Hub // Trung tâm kết nối
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message); // Gửi cho tất cả Client, ReceiveMessage: Client phải đặt tên hàm như này để nhận dữ liệu
        }
    }
}
