using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace Server
{
    public class ChatHub : Hub // Trung tâm kết nối
    {

        public async Task GuiTN(string user, string mess) // Client phải đặt tên hàm như này để gửi cho server
        {
            await Clients.All.SendAsync("NhanTN", user, mess); // Gửi cho tất cả Client, ReceiveMessage: Client phải đặt tên hàm như này để nhận dữ liệu
        }

        public async Task TrangThaiTK(string mess)
        {
            await Clients.All.SendAsync("ThongBaoTK", mess);
        }
    }
}
