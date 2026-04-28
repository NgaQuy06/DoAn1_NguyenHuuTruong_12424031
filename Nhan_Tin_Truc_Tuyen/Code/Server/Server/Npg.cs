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
            catch { return list; }

            return list;
        }

        public static void TinNhan(string username, string message)
        {
            NpgsqlConnection conn;
            try
            {
                conn = new NpgsqlConnection(str);
                conn.Open();
                string sql = "INSERT INTO public.\"TinNhan\" (\"MaTK\", \"MaCTC\", \"NoiDung\", \"NgayGui\") VALUES (@a, @b, @c, @d)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("a", username);
                    cmd.Parameters.AddWithValue("b", 0);
                    cmd.Parameters.AddWithValue("c", message.Trim());
                    cmd.Parameters.AddWithValue("d", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi tin nhắn: " + ex.Message);
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
                Console.WriteLine("Lỗi cập nhật trạng thái: " + ex.Message);
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
                Console.WriteLine("Lỗi đếm tài khoản: " + ex.Message);
            }
            return "0";
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
                Console.WriteLine("Lỗi đếm tin nhắn: " + ex.Message);
            }
            return "0";
        }
    }

    public class ThongTinBB
    {
        public string TenTK { get; set; }
        public string BietDanh { get; set; }
        public string TrangThai { get; set; }
    }
}
