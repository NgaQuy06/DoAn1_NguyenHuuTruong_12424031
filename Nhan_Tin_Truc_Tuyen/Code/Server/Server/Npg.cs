using Microsoft.AspNetCore.SignalR;
using Npgsql;
using System.Data;

namespace Server
{
    public class Npg
    {
        public static string str = "Host=aws-0-ap-southeast-1.pooler.supabase.com;" +
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
                string sql = "SELECT \"TenTK\", \"BietDanh\", \"TrangThai\" FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @u";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", username.Trim());
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
            catch { }

            return list;
        }
    }

    public class ThongTinBB
    {
        public string TenTK { get; set; }
        public string BietDanh { get; set; }
        public string TrangThai { get; set; }
    }
}
