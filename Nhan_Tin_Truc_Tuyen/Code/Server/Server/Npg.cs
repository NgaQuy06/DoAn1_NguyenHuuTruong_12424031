using Microsoft.AspNetCore.SignalR;
using Npgsql;
using System.Data;

namespace Server
{
    public class Npg
    {
        public static string str = "Host=aws-1-ap-northeast-1.pooler.supabase.com;" +
                   "Port=6543;" +
                   "Database=postgres;" +
                   "Username=postgres.fauxrzhhtdiesxfxuftz;" +
                   "Password=Nguyentrg2006$;" +
                   "SSL Mode=Require;" +
                   "Trust Server Certificate=true;";

        public static ThongTinDN DangNhap(string username, string password, string role)
        {
            ThongTinDN dn = new ThongTinDN();
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "select * from fun_dangnhaptaikhoan(@u, @p, @r)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", username.Trim());
                    cmd.Parameters.AddWithValue("p", password.Trim());
                    cmd.Parameters.AddWithValue("r", role.Trim());
                    using var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        dn.MaTK = reader.IsDBNull(0) ? -1 : reader.GetInt64(0);
                        dn.Email = reader.IsDBNull(1) ? "" : reader.GetString(1);
                        dn.BietDanh = reader.IsDBNull(2) ? "" : reader.GetString(2);
                        return dn;
                    }
                    return null;
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine("Lỗi đăng nhập: " + e.Message);
                return null;
            }
        }

        public static string KiemTraTrangThai(string tenTK)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT \"TrangThai\" FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @t";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", tenTK.Trim());
                    var reader = cmd.ExecuteScalar();
                    if (reader != null)
                    {
                        return reader.ToString();
                    }
                    return "";
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine("Lỗi kiểm tra trạng thái: " + e.Message);
                throw;
            }
        }

        public static string DangKy(string username, string password, string email)
        {
            if (KiemTraTenTK(username))
            {
                return "Tên tài khoản đã tồn tại!";
            }

            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "INSERT INTO public.\"TaiKhoan\" (\"TenTK\", \"MatKhau\", \"Email\", \"TrangThai\", \"BietDanh\", \"NgayTao\", \"QuyenHan\") VALUES (@a, @b, @c, @d, @e, @f, @g)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("a", username.Trim());
                    cmd.Parameters.AddWithValue("b", password.Trim());
                    if (string.IsNullOrWhiteSpace(email))
                        cmd.Parameters.AddWithValue("c", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("c", email.Trim());
                    cmd.Parameters.AddWithValue("d", "Đang ngoại tuyến");
                    cmd.Parameters.AddWithValue("e", "Người dùng mới");
                    cmd.Parameters.AddWithValue("f", DateTime.Now);
                    cmd.Parameters.AddWithValue("g", "NguoiDung");
                    int reader = cmd.ExecuteNonQuery();
                    if (reader > 0)
                    {
                        return "Ok";
                    }
                    else
                    {
                        return "Đăng ký thất bại";
                    }
                }
            }
            catch (NpgsqlException e)
            {
                return "Lỗi đăng ký: " + e.Message;
            }
        }

        public static bool KiemTraTenTK(string username)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT * FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @u";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", username.Trim());
                    var reader = cmd.ExecuteScalar();
                    if (reader != null)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine("Lỗi kiểm tra tên tài khoản: " + e.Message);
                return false;
            }
        }

        public static List<ThongTinBB> TimKiemBB(string username)
        {
            var list = new List<ThongTinBB>();
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT \"TenTK\", \"BietDanh\", \"TrangThai\" FROM public.\"TaiKhoan\" WHERE \"TenTK\" ILIKE @u";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", "%" + username.Trim() + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ThongTinBB
                            {
                                TenTK = reader["TenTK"].ToString(),
                                BietDanh = reader["BietDanh"].ToString(),
                                TrangThai = reader["TrangThai"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi DB(tìm kiếm bạn bè): " + ex.Message);
            }

            return list;
        }

        public static void ChenTNChung(string username, string message)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();

                int maTK = -1;
                string sql1 = "SELECT \"MaTK\" FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @u";
                using (var cmd1 = new NpgsqlCommand(sql1, conn))
                {
                    cmd1.Parameters.AddWithValue("u", username.Trim());
                    var reader = cmd1.ExecuteScalar();
                    if (reader == null)
                    {
                        Console.WriteLine("Tài khoản không tồn tại: " + username);
                        return;
                    }
                    else
                    {
                        maTK = Convert.ToInt32(reader);
                    }
                }

                string sql2 = "INSERT INTO public.\"TinNhan\" (\"MaTK\", \"MaCTC\", \"NoiDung\", \"NgayGui\", \"TenTK\") VALUES (@a, @b, @c, @d, @e)";
                using (var cmd2 = new NpgsqlCommand(sql2, conn))
                {
                    cmd2.Parameters.AddWithValue("a", maTK);
                    cmd2.Parameters.AddWithValue("b", 0);
                    cmd2.Parameters.AddWithValue("c", message.Trim());
                    cmd2.Parameters.AddWithValue("d", DateTime.Now);
                    cmd2.Parameters.AddWithValue("e", username.Trim());
                    cmd2.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tin nhắn): " + ex.Message);
                throw;
            }
        }

        public static void ChenTNRieng(int maTK, string ngGui, int maCTC, string mess)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "INSERT INTO public.\"TinNhan\" (\"MaTK\", \"MaCTC\", \"NoiDung\", \"NgayGui\", \"TenTK\") VALUES (@a, @b, @c, @d, @e)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("a", maTK);
                    cmd.Parameters.AddWithValue("b", maCTC);
                    cmd.Parameters.AddWithValue("c", mess.Trim());
                    cmd.Parameters.AddWithValue("d", DateTime.Now);
                    cmd.Parameters.AddWithValue("e", ngGui.Trim());
                    cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tin nhắn riêng): " + ex.Message);
                throw;
            }
        }

        public static void CapNhatTrangThai(string username, string trangThai)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "UPDATE public.\"TaiKhoan\" SET \"TrangThai\" = @tt, \"ThoiGianHDGanDay\" = @tg WHERE \"TenTK\" = @t";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", username);
                    cmd.Parameters.AddWithValue("tt", trangThai);
                    cmd.Parameters.AddWithValue("tg", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(cập nhật trạng thái): " + ex.Message);
            }
        }

        public static string TongTK()
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT COUNT(*) FROM public.\"TaiKhoan\"";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    var reader = cmd.ExecuteScalar();
                    return reader.ToString();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tổng tài khoản): " + ex.Message);
            }
            return "1";
        }

        public static string TongTN()
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT COUNT(*) FROM public.\"TinNhan\"";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    var reader = cmd.ExecuteScalar();
                    return reader.ToString();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tổng tin nhắn): " + ex.Message);
            }
            return "1";
        }

        public static List<ThongTinTK> ThongTinTK()
        {
            var list = new List<ThongTinTK>();
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT \"TenTK\", \"MatKhau\", \"Email\", \"TrangThai\", \"BietDanh\", \"NgayTao\" FROM public.\"TaiKhoan\"";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ThongTinTK
                            {
                                TenTK = reader.GetString(0),
                                MatKhau = reader.GetString(1),
                                Email = reader.IsDBNull(2) ? null : reader.GetString(2),
                                TrangThai = reader.GetString(3),
                                BietDanh = reader.IsDBNull(4) ? null : reader.GetString(4),
                                NgayTao = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(thông tin tài khoản): " + ex.Message);
                throw;
            }

            return list;
        }

        public static List<TinNhanDienDan> TinNhanDienDan()
        {
            var list = new List<TinNhanDienDan>();
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT tn.\"TenTK\", tn.\"NoiDung\", tn.\"NgayGui\", tk.\"BietDanh\" FROM public.\"TinNhan\" tn JOIN public.\"TaiKhoan\" tk ON tn.\"TenTK\" = tk.\"TenTK\" WHERE tn.\"MaCTC\" = 0 ORDER BY tn.\"NgayGui\" ASC LIMIT 50";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TinNhanDienDan
                            {
                                TenTK = reader.IsDBNull(0) ? "" : reader.GetString(0),
                                NoiDung = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                NgayGui = reader.IsDBNull(2) ? DateTime.Now : reader.GetDateTime(2),
                            });
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tin nhắn diễn đàn): " + ex.Message);
                throw;
            }
            return list;
        }

        public static List<TinNhanRieng> TinNhanRieng(string user)
        {
            var list = new List<TinNhanRieng>();
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT ctc.\"MaCTC\", ctc.\"TenCTC\", tn.\"TenTK\", tn.\"NoiDung\", tn.\"NgayGui\" FROM public.\"CuocTroChuyen\" ctc JOIN public.\"ThanhVienNhom\" tvn ON ctc.\"MaCTC\" = tvn.\"MaCTC\" LEFT JOIN public.\"TinNhan\" tn ON ctc.\"MaCTC\" = tn.\"MaCTC\" WHERE tvn.\"TenTK\" = @user AND ctc.\"MaCTC\" <> 0 AND ctc.\"TrangThai\" <> 'Đã bị xóa' ORDER BY tn.\"NgayGui\" ASC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("user", user.Trim());
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TinNhanRieng
                            {
                                MaCTC = reader.IsDBNull(0) ? -1 : reader.GetInt16(0),
                                TenCTC = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                TenTK = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                NoiDung = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                NgayGui = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tin nhắn riêng): " + ex.Message);
                throw;
            }
            return list;
        }

        public static int SoLuongTrucTuyen()
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT COUNT(*) FROM public.\"TaiKhoan\" WHERE \"TrangThai\" = 'Đang trực tuyến'";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    var reader = cmd.ExecuteScalar();
                    return Convert.ToInt32(reader);
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(số lượng trực tuyến): " + ex.Message);
            }
            return 0;
        }

        public static void CamTaiKhoan(string tenTK)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "UPDATE public.\"TaiKhoan\" SET \"TrangThai\" = 'Đã bị cấm' WHERE \"TenTK\" = @t";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", tenTK);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(cấm tài khoản): " + ex.Message);
            }
        }

        public static int TaoCTC(string tenCTC)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql1 = "INSERT INTO public.\"CuocTroChuyen\" (\"TenCTC\", \"NgayTao\", \"TrangThai\") VALUES (@t, @n, @r) RETURNING \"MaCTC\"";
                using (var cmd = new NpgsqlCommand(sql1, conn))
                {
                    cmd.Parameters.AddWithValue("t", tenCTC);
                    cmd.Parameters.AddWithValue("n", DateTime.Now);
                    cmd.Parameters.AddWithValue("r", "Bình thường");
                    var reader = cmd.ExecuteScalar();
                    if (reader != null)
                    {
                        return Convert.ToInt32(reader);
                    }
                    else
                    {
                        Console.WriteLine("Không thể tạo cuộc trò chuyện mới");
                        return -1;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tạo cuộc trò chuyện): " + ex.Message);
            }
            return -1;
        }

        public static void SuaCTC(int maCTC, string tenCTC)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "update public.\"CuocTroChuyen\" set \"TenCTC\" = @tenCTC where \"MaCTC\" = @maCTC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("tenCTC", tenCTC);
                    cmd.Parameters.AddWithValue("maCTC", maCTC);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(sửa cuộc trò chuyện): " + ex.Message);
            }
        }

        public static void XoaCTC(int maCTC)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "update public.\"CuocTroChuyen\" set \"TrangThai\" = 'Đã bị xóa' where \"MaCTC\" = @maCTC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("maCTC", maCTC);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(xóa cuộc trò chuyện): " + ex.Message);
            }
        }

        public static void ThemThanhVien(int maCTC, string tenTK)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "INSERT INTO public.\"ThanhVienNhom\" (\"MaCTC\", \"MaTK\", \"TenTK\", \"NgayTG\") VALUES (@m, @ma, @t, @n)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("m", maCTC);
                    cmd.Parameters.AddWithValue("ma", LayMaTK(tenTK));
                    cmd.Parameters.AddWithValue("t", tenTK.Trim());
                    cmd.Parameters.AddWithValue("n", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(thêm thành viên): " + ex.Message);
            }
        }

        public static int LayMaTK(string tenTK)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT \"MaTK\" FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @t";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", tenTK.Trim());
                    var reader = cmd.ExecuteScalar();
                    if (reader != null)
                    {
                        return Convert.ToInt32(reader);
                    }
                    else
                    {
                        Console.WriteLine("Tài khoản không tồn tại: " + tenTK);
                        return -1;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(lấy mã tài khoản): " + ex.Message);
            }
            return -1;
        }

        public static bool KiemTraKetBan(int maTK1, int maTK2)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "select * from public.\"BanBe\" where ((\"maTK1\" = @maTK1 AND \"maTK2\" = @maTK2) OR (\"maTK1\" = @maTK2 AND \"maTK2\" = @maTK1)) and TrangThai = 'Đang chờ'";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("maTK1", maTK1);
                    cmd.Parameters.AddWithValue("maTK2", maTK2);
                    var a = cmd.ExecuteScalar();
                    if (a != null) return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi DB(kiểm tra kết bạn): " + ex.Message);
                throw;
            }
        }

        public static void KetBan(int maTK1, int maTK2)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "INSERT INTO public.\"BanBe\" (\"TenTK1\", \"TenTK2\", \"TrangThai\", \"NgayTG\") VALUES (@t1, @t2, @t, @n)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t1", maTK1);
                    cmd.Parameters.AddWithValue("t2", maTK2);
                    cmd.Parameters.AddWithValue("t", "Đang chờ");
                    cmd.Parameters.AddWithValue("n", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(kết bạn): " + ex.Message);
            }
        }

        public static List<ThongTinBanBe> LayDanhSachBanBe(int maTK)
        {
            var bb = new List<ThongTinBanBe>();
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT tk.\"MaTK\", tk.\"TenTK\", tk.\"BietDanh\", bb.\"TrangThai\" FROM public.\"BanBe\" bb JOIN public.\"TaiKhoan\" tk ON (bb.\"TenTK1\" = tk.\"MaTK\" OR bb.\"TenTK2\" = tk.\"MaTK\") WHERE (bb.\"TenTK1\" = @maTK OR bb.\"TenTK2\" = @maTK) AND bb.\"TrangThai\" = 'Đang chờ'";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("maTK", maTK);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            bb.Add(new ThongTinBanBe
                            {
                                MaTK = reader.IsDBNull(0) ? -1 : reader.GetInt32(0),
                                TenTK = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                BietDanh = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                TrangThai = reader.IsDBNull(3) ? "" : reader.GetString(3)
                            });
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(thông tin bạn bè): " + ex.Message);
                throw;
            }
            return bb;
        }

        public static List<string> LayLoiMoiKetBan(int maTK)
        {
            var loiMoi = new List<string>();
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT \"MaNgGui\" FROM public.\"BanBe\" WHERE \"MaNgNhan\" = @maTK AND \"TrangThai\" = 'Đang chờ'";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("maTK", maTK);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            loiMoi.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(lời mời kết bạn): " + ex.Message);
            }
            return loiMoi;
        }
    }

    public class ThongTinDN
    {
        public long MaTK { get; set; }
        public string Email { get; set; }
        public string BietDanh { get; set; }
    }

    public class ThongTinBB
    {
        public string TenTK { get; set; }
        public string BietDanh { get; set; }
        public string TrangThai { get; set; }
    }

    public class ThongTinTK
    {
        public string TenTK { get; set; }
        public string MatKhau { get; set; }
        public string Email { get; set; }
        public string TrangThai { get; set; }
        public string BietDanh { get; set; }
        public DateTime NgayTao { get; set; }
    }

    public class TinNhanDienDan
    {
        public string TenTK { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayGui { get; set; }
    }

    public class TinNhanRieng
    {
        public int MaCTC { get; set; }
        public string TenCTC { get; set; }
        public string TenTK { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayGui { get; set; }
    }

    public class ThongTinBanBe
    {
        public int MaTK { get; set; }
        public string TenTK { get; set; }
        public string BietDanh { get; set; }
        public string TrangThai { get; set; }
    } 
}
