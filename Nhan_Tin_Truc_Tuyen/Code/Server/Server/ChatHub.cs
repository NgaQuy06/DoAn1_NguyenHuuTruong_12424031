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
                Npg.ChenTNChung(user, mess);
                await Clients.All.SendAsync("NhanTNChung", user, mess); // Gửi cho tất cả Client, ReceiveMessage: Client phải đặt tên hàm như này để nhận dữ liệu
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message); // Gửi lỗi về cho Client gọi hàm này
            }
        }

        public async Task GuiTNRieng(string fromUser, string toUser, string mess)
        {
            try
            {
                //Npg.ChenTinNhan(fromUser, mess);

                // gửi cho người nhận
                await Clients.User(toUser).SendAsync("NhanTNRieng", fromUser, mess);

                // gửi lại cho chính mình (để hiển thị)
                await Clients.Caller.SendAsync("NhanTNRieng", fromUser, mess);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
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
        
        public async Task SoLuongTrucTuyen()
        {
            try
            {
                int count = Npg.SoLuongTrucTuyen();
                await Clients.Caller.SendAsync("SoLuongTrucTuyen", count);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("User: " + Context.UserIdentifier);
            await base.OnConnectedAsync();
        }
    }

    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext().Request.Query["username"];
        }
    }
}
