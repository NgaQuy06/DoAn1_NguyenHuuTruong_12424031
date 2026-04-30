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

        public static string DangNhap(string username, string password, string role)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "SELECT * FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @u AND \"MatKhau\" = @p AND \"QuyenHan\" = @r";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", username.Trim());
                    cmd.Parameters.AddWithValue("p", password.Trim());
                    cmd.Parameters.AddWithValue("r", role.Trim());
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return "Ok";
                    }
                    else
                    {
                        return "Sai tên tài khoản hoặc mật khẩu";
                    }
                }
            }
            catch (Exception e)
            {
                return "Lỗi đăng nhập: " + e.Message;
            }
        }

        public static string DangKy(string username, string password, string email)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "INSERT INTO public.\"TaiKhoan\" (\"TenTK\", \"MatKhau\", \"Email\", \"TrangThai\", \"BietDanh\", \"NgayTao\") VALUES (@a, @b, @c, @d, @e, @f)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("a", username.Trim());
                    cmd.Parameters.AddWithValue("b", password.Trim());
                    if (string.IsNullOrWhiteSpace(email))
                        cmd.Parameters.AddWithValue("c", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("c", email.Trim());
                    cmd.Parameters.AddWithValue("d", "Không hoạt động");
                    cmd.Parameters.AddWithValue("e", "Người dùng mới");
                    cmd.Parameters.AddWithValue("f", DateTime.Now);
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
            catch (Exception e)
            {
                return "Lỗi đăng ký: " + e.Message;
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

        public static void ChenTinNhan(string username, string message)
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
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi DB(tin nhắn): " + ex.Message);
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
                string sql = "UPDATE public.\"TaiKhoan\" SET \"TrangThai\" = @trangThai WHERE \"TenTK\" = @t";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", username);
                    cmd.Parameters.AddWithValue("trangThai", trangThai);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
                string sql = "SELECT \"TenTK\", \"NoiDung\", \"NgayGui\", \"BietDanh\" FROM public.\"TinNhan\" tn JOIN public.\"TaiKhoan\" tk ON tn.\"TenTK\" = tk.\"TenTK\" WHERE \"MaCTC\" = 0 ORDER BY \"NgayGui\" ASC";
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
                                BietDanh = reader.IsDBNull(3) ? "" : reader.GetString(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi DB(tin nhắn diễn đàn): " + ex.Message);
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
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi DB(số lượng trực tuyến): " + ex.Message);
            }
            return 0;
        }
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
        public string BietDanh { get; set; }
    }
}
