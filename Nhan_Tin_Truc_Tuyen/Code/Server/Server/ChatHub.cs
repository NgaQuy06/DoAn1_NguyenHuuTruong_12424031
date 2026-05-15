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

        public async Task GuiTNRieng(int maCTC, int maTK, string tenTK, List<string> dsNgNhan, string mess)
        {
            try
            {
                await Npg.ChenTNRieng(maTK, tenTK, maCTC, mess);
                // gửi cho người nhận
                foreach (string ngNhan in dsNgNhan.Distinct())
                {
                    if (ngNhan == tenTK)
                        continue;

                    await Clients.User(ngNhan).SendAsync("NhanTNRieng", new TNRieng { MaCTC = maCTC, TenTK = tenTK, NoiDung = mess, NgayGui = DateTime.Now });
                }
                // gửi lại cho chính mình (để hiển thị)
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

        public async Task TaoCTC(string tenCTC, string tenTK, List<string> dsNgNhan)
        {
            try
            {
                int maCTC = await Npg.TaoCTC(tenCTC);
                if (maCTC != -1)
                {
                    await Npg.ThemThanhVien(maCTC, tenTK);
                    foreach (string ngNhan in dsNgNhan.Distinct())
                    {

                        await Npg.ThemThanhVien(maCTC, ngNhan);
                        await Clients.User(ngNhan).SendAsync("ThemCTC", maCTC, tenCTC, dsNgNhan);
                    }
                    await Clients.Caller.SendAsync("ThemCTC", maCTC, tenCTC, dsNgNhan);
                    await Clients.Caller.SendAsync("ThongBaoCTC", "Bạn đã được " + tenTK + "Tạo nhóm chat thành công!");
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

        public async Task GuiLoiMoiKetBan(string tenTK1, string tenTK2)
        {
            try
            {
                int maTK1 = await Npg.LayMaTK(tenTK1);
                int maTK2 = await Npg.LayMaTK(tenTK2);
                await Npg.KetBan(maTK1, maTK2);
                await Clients.User(tenTK2).SendAsync("NhanLoiMoiKetBan", tenTK1);
                await Clients.Caller.SendAsync("ThongBaoKetBan", "Đã gửi lời mời kết bạn đến " + tenTK2);
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
