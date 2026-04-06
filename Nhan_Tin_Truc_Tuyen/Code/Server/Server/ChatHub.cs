using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace Server
{
    public class ChatHub : Hub // Trung tâm kết nối
    {
        string str = "Host=aws-1-ap-northeast-1.pooler.supabase.com;" +
                     "Port=6543;" +
                     "Database=postgres;" +
                     "Username=postgres.fauxrzhhtdiesxfxuftz;" +
                     "Password=Nguyentrg2006$;" +
                     "SSL Mode=Require;" +
                     "Trust Server Certificate=true;";

        public async Task SendMessage(string user, string message) // Client phải đặt tên hàm như này để gửi cho server
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message); // Gửi cho tất cả Client, ReceiveMessage: Client phải đặt tên hàm như này để nhận dữ liệu
        }
        public async Task Logout(string user)
        {
            
            await Clients.Caller.SendAsync("Logout", mess);
        }
    }
}
