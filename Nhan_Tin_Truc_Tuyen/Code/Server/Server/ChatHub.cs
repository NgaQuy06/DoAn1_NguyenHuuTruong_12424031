using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace Server
{
    public class ChatHub : Hub // Trung tâm kết nối
    {
        public async Task GuiTNChung(string user, string mess) // Client phải đặt tên hàm như này để gửi cho server
        {
            try
            { 
                Npg.ChenTinNhan(user, mess);
                await Clients.All.SendAsync("NhanTN", user, mess); // Gửi cho tất cả Client, ReceiveMessage: Client phải đặt tên hàm như này để nhận dữ liệu
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message); // Gửi lỗi về cho Client gọi hàm này
            }
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

        public async Task DaThamGia(string username)
        {
            Npg.CapNhatTrangThai(username, "Đang trực tuyến");
            await Clients.All.SendAsync("ThongBaoTK", username + " đã trực tuyến!");
        }

        public async Task DaRoiKhoi(string username)
        {
            Npg.CapNhatTrangThai(username, "Đang ngoại tuyến");
            await Clients.All.SendAsync("ThongBaoTK", username + " đã ngoại tuyến!");
        }

        public async Task TongTKTN()
        {
            try
            {
                int tk = int.Parse(Npg.TongTK());
                int tn = int.Parse(Npg.TongTN());
                await Clients.Caller.SendAsync("TongTKTN", tk, tn);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task ThongTinTK()
        {
            try
            {
                var list = Npg.ThongTinTK();
                await Clients.Caller.SendAsync("ThongTinTK", list);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task TinNhanDienDan()
        {
            try
            {
                var list = Npg.TinNhanDienDan();
                await Clients.Caller.SendAsync("TinNhanDienDan", list);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }
    }
}
