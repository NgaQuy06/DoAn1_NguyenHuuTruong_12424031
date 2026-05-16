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
                await Npg.ChenTNChung(user, mess);
                await Clients.All.SendAsync("NhanTNChung", user, mess); // Gửi cho tất cả Client, ReceiveMessage: Client phải đặt tên hàm như này để nhận dữ liệu
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message); // Gửi lỗi về cho Client gọi hàm này
            }
        }

        public async Task GuiTNRieng(int maCTC, int maTK, string tenTK, string tenNgNhan, string mess)
        {
            try
            {
                await Npg.ChenTNRieng(maTK, tenTK, maCTC, mess);
                await Clients.User(tenNgNhan).SendAsync("NhanTNRieng", new TNRieng { MaCTC = maCTC, TenTK = tenTK, NoiDung = mess, NgayGui = DateTime.Now });
                await Clients.Caller.SendAsync("NhanTNRieng", new TNRieng { MaCTC = maCTC, TenTK = tenTK, NoiDung = mess, NgayGui = DateTime.Now } );
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task TrangThaiTK(string mess) // trạng thái tài khoản
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
            await Npg.CapNhatTrangThai(username, "Đang trực tuyến");
            int sl = await Npg.SoLuongTrucTuyen();
            await Clients.All.SendAsync("ThongBaoTK", username + " đã trực tuyến!");
            await Clients.All.SendAsync("SoLuongTrucTuyen", sl);
        }

        public async Task DaRoiKhoi(string username)
        {
            await Npg.CapNhatTrangThai(username, "Đang ngoại tuyến");
            int sl = await Npg.SoLuongTrucTuyen();
            await Clients.All.SendAsync("ThongBaoTK", username + " đã ngoại tuyến!");
            await Clients.All.SendAsync("SoLuongTrucTuyen", sl);
        }

        public async Task TongTKTN()
        {
            try
            {
                int tk = int.Parse(await Npg.TongTK());
                int tn = int.Parse(await Npg.TongTN());
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
                var list = await Npg.ThongTinTK();
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
                int count = await Npg.SoLuongTrucTuyen();
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
                await Npg.CamTaiKhoan(tenTK);
                await Clients.Caller.SendAsync("ThongBaoTuQTV", "Tài khoản đã bị cấm!");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task TaoCTC(string tenCTC, string tenTK, string tenNgNhan)
        {
            try
            {
                int maCTC = await Npg.TaoCTC(tenCTC);
                if (maCTC != -1)
                {
                    await Npg.ThemThanhVien(maCTC, tenTK);
                    await Npg.ThemThanhVien(maCTC, tenNgNhan);
                    await Clients.User(tenNgNhan).SendAsync("ThemCTC", maCTC, tenCTC, tenNgNhan);
                    await Clients.Caller.SendAsync("ThemCTC", maCTC, tenCTC, tenNgNhan);
                    await Clients.Caller.SendAsync("ThongBaoCTC", "Tạo nhóm chat thành công!");
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.ToString());
            }
        }

        public async Task SuaCTC(int maCTC, string tenCTC, string tenTK)
        {
            try
            {
                await Npg.SuaCTC(maCTC, tenCTC);
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
                await Npg.XoaCTC(maCTC);
                await Clients.Caller.SendAsync("ThongBaoCTC", "Xóa thành công!");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task GuiLoiMoiKetBan(string tenNgGui, string tenNgNhan)
        {
            try
            {
                if (await Npg.KiemTraTrangThaiKetBan(tenNgGui, "Đang chờ"))
                {
                    await Clients.Caller.SendAsync("ThongBaoKetBan", "Bạn đã gửi lời mời kết bạn cho " + tenNgNhan + " rồi, vui lòng chờ phản hồi!");
                    return;
                }
                int maNgGui = await Npg.LayMaTK(tenNgGui);
                int maNgNhan = await Npg.LayMaTK(tenNgNhan);
                await Npg.KetBan(maNgGui, maNgNhan);
                await Clients.User(tenNgNhan).SendAsync("NhanLoiMoiKetBan", tenNgGui);
                await Clients.Caller.SendAsync("ThongBaoKetBan", "Đã gửi lời mời kết bạn thành công");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Loi", ex.Message);
            }
        }

        public async Task TraLoiKetBan(string tl, string tenTK, string ngGui)
        {
            try
            {
                await Npg.TraLoiKetBan(tl, tenTK, ngGui);
                if (tl == "Kết bạn thành công")
                {
                    await Clients.User(ngGui).SendAsync("ThongBaoKetBan", tenTK + " đã chấp nhận lời mời kết bạn của bạn!");
                    var tb = await Npg.ThemBanBe(tenTK);
                    await Clients.User(ngGui).SendAsync("ThemBanBe", tb);
                }
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
