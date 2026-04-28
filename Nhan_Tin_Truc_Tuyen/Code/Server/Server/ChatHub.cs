using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace Server
{
    public class ChatHub : Hub // Trung tâm kết nối
    {
        public async Task GuiTNChung(string user, string mess) // Client phải đặt tên hàm như này để gửi cho server
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(Npg.str);
                conn.Open();
                string sql = "INSERT INTO public.\"TinNhan\" ()";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    var reader = cmd.ExecuteReader();
                }
                await Clients.All.SendAsync("NhanTN", user, mess); // Gửi cho tất cả Client, ReceiveMessage: Client phải đặt tên hàm như này để nhận dữ liệu
            }
            catch { }
        }

        public async Task GuiTNRieng(string fromUser, string toUser, string mess)
        {
            await Clients.Group(toUser).SendAsync("NhanTN", fromUser, mess);
        }

        public async Task TrangThaiTK(string mess)
        {
            await Clients.All.SendAsync("ThongBaoTK", mess);
        }

        public async Task TimKiemBB(string username)
        {
            var list = Npg.TimKiemBB(username);
            await Clients.Caller.SendAsync("TimKiemBB", list);
        }
    }
}
