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

        public async Task GuiTNRieng(int maTK, string ngGui, int maCTC, string ngNhan, string mess)
        {
            try
            {
                Console.WriteLine($"GuiTNRieng: maTK={maTK}, ngGui={ngGui}, maCTC={maCTC}, ngNhan={ngNhan}, mess={mess}");
                Npg.ChenTNRieng(maTK, ngGui, maCTC, mess);
                // gửi cho người nhận
                await Clients.User(ngNhan).SendAsync("NhanTNRieng", new TNRieng { MaCTC = maCTC, TenCTC = "",TenTK = ngGui, NoiDung = mess, NgayGui = DateTime.Now } );

                // gửi lại cho chính mình (để hiển thị)
                await Clients.Caller.SendAsync("NhanTNRieng", new TNRieng { MaCTC = maCTC, TenCTC = "", TenTK = ngGui, NoiDung = mess, NgayGui = DateTime.Now } );
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

        public async Task CamTaiKhoan(string tenTK)
        {
            try
            {
                Npg.CamTaiKhoan(tenTK);
                await Clients.Caller.SendAsync("ThongBaoTuQTV", "Tài khoản đã bị cấm!");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task TaoCTC(string tenCTC, string tenTK, string ngNhan)
        {
            try
            {
                int maCTC = Npg.TaoCTC(tenCTC);
                if (maCTC != -1)
                {
                    Npg.ThemThanhVien(maCTC, ngNhan);
                    await Clients.Caller.SendAsync("ThemCTC", maCTC, tenCTC, ngNhan);
                    await Clients.User(ngNhan).SendAsync("ThemCTC", maCTC, tenCTC, tenTK);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task SuaCTC(int maCTC, string tenCTC, string tenTK)
        {
            try
            {
                Npg.SuaCTC(maCTC, tenCTC);
                await Clients.Caller.SendAsync("ThongBaoCTC", "Sửa thành công!");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task XoaCTC(int maCTC, string tenTK)
        {
            try
            {
                Npg.XoaCTC(maCTC);
                await Clients.Caller.SendAsync("ThongBaoCTC", "Xóa thành công!");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task GuiLoiMoiKetBan(string tenTK1, string tenTK2)
        {
            try
            {
                int maTK1 = Npg.LayMaTK(tenTK1);
                int maTK2 = Npg.LayMaTK(tenTK2);
                Npg.KetBan(maTK1, maTK2);
                await Clients.User(tenTK2).SendAsync("NhanLoiMoiKetBan", tenTK1);
                await Clients.Caller.SendAsync("GuiLoiMoiKetBan", "Đã gửi lời mời kết bạn đến " + tenTK2);
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task TraLoiKetBan(string tl, string tenTK, string ngNhan)
        {
            try
            {

            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine(Context.UserIdentifier + " đã kết nối đến máy chủ vào lúc " + DateTime.Now);
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine(Context.UserIdentifier + " đã rời khỏi máy chủ vào lúc " + DateTime.Now);
            return base.OnDisconnectedAsync(exception);
        }
    }

    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.GetHttpContext().Request.Query["username"];
        }
    }

    public class TNRieng
    {
        public int MaCTC { get; set; }
        public string TenCTC { get; set; }
        public string TenTK { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayGui { get; set; }
    }

    public class ThemCTC
    {
        public int MaCTC { get; set; }
        public string TenCTC { get; set; }
        public string TenTK { get; set; }
    }
}
