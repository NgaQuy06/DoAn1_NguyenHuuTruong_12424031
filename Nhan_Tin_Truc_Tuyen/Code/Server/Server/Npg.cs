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
                   "Trust Server Certificate=true;" +
                   "Pooling=true;" +
                   "Minimum Pool Size=5;" +
                   "Maximum Pool Size=50;" +
                   "Timeout=15;" +
                   "Command Timeout=30;";

        public static async Task<ThongTinDN?> DangNhap(string username, string password, string role)
        {
            ThongTinDN dn = new ThongTinDN();
            //string sql = "select 1 from fun_dangnhaptaikhoan(@u, @p, @r)";
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "select \"MaTK\", \"Email\", \"BietDanh\" from public.\"TaiKhoan\" where \"TenTK\" = @u and \"MatKhau\" = @p and \"QuyenHan\" = @r";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", username.Trim());
                    cmd.Parameters.AddWithValue("p", password.Trim());
                    cmd.Parameters.AddWithValue("r", role.Trim());
                    using var reader = await cmd.ExecuteReaderAsync();

                    if (await reader.ReadAsync())
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
                Console.WriteLine("Lỗi DB(đăng nhập): " + e.Message);
                return null;
            }
        }

        public async static Task<string> KiemTraTrangThai(string tenTK)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "SELECT \"TrangThai\" FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @t ";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", tenTK.Trim());
                    var reader = await cmd.ExecuteScalarAsync();
                    if (reader != null)
                    {
                        return reader?.ToString() ?? "";
                    }
                    return "";
                }
            }
            catch (NpgsqlException e)
            {
                Console.WriteLine("Lỗi DB(kiểm tra trạng thái): " + e.Message);
                throw;
            }
        }

        public async static Task<string> DangKy(string TenTK, string MatKhau, string Email, int Sdt)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "INSERT INTO public.\"TaiKhoan\" (\"TenTK\", \"MatKhau\", \"Email\", \"Sdt\", \"TrangThai\", \"BietDanh\", \"NgayTao\", \"QuyenHan\") VALUES (@a, @b, @c, @d, @e, @f, @g, @h)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("a", TenTK.Trim());
                    cmd.Parameters.AddWithValue("b", MatKhau.Trim());
                    if (string.IsNullOrWhiteSpace(Email))
                        cmd.Parameters.AddWithValue("c", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("c", Email.Trim());
                    cmd.Parameters.AddWithValue("d", Sdt);
                    cmd.Parameters.AddWithValue("e", "Đang ngoại tuyến");
                    cmd.Parameters.AddWithValue("f", "Người dùng mới");
                    cmd.Parameters.AddWithValue("g", DateTime.Now);
                    cmd.Parameters.AddWithValue("h", "NguoiDung");
                    int reader = await cmd.ExecuteNonQueryAsync();
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

        public static async Task<bool> XacMinhTaiKhoan(string token)
        {
            using var client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "secret", "6LfUTessAAAAAMXT8yNhWOABi-YeAnWi3LE6Jq7n" },
                { "response", token }
            };
            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            var json = await response.Content.ReadAsStringAsync();
            return json.Contains("\"success\": true");
        }

        public async static Task<bool> KiemTraTenTK(string username)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "SELECT 1 FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @u ";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", username.Trim());
                    var reader = await cmd.ExecuteScalarAsync();
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

        public async static Task<List<ThongTinBB>> TimKiemBB(string username)
        {
            var list = new List<ThongTinBB>();
            try
            {
                using var conn = new NpgsqlConnection(str); 
                await conn.OpenAsync();
                string sql = "SELECT \"TenTK\", \"BietDanh\", \"TrangThai\" FROM public.\"TaiKhoan\" WHERE \"TenTK\" ILIKE @u";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("u", "%" + username.Trim() + "%");
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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

        public async static Task ChenTNChung(string username, string message)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                int maTK = -1;
                string sql1 = "SELECT \"MaTK\" FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @u ";
                using (var cmd1 = new NpgsqlCommand(sql1, conn))
                {
                    cmd1.Parameters.AddWithValue("u", username.Trim());
                    var reader = await cmd1.ExecuteScalarAsync();
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
                    await cmd2.ExecuteNonQueryAsync();
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tin nhắn): " + ex.Message);
                throw;
            }
        }

        public async static Task ChenTNRieng(int maTK, string ngGui, int maCTC, string mess)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "INSERT INTO public.\"TinNhan\" (\"MaTK\", \"MaCTC\", \"NoiDung\", \"NgayGui\", \"TenTK\") VALUES (@a, @b, @c, @d, @e)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("a", maTK);
                    cmd.Parameters.AddWithValue("b", maCTC);
                    cmd.Parameters.AddWithValue("c", mess.Trim());
                    cmd.Parameters.AddWithValue("d", DateTime.Now);
                    cmd.Parameters.AddWithValue("e", ngGui.Trim());
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tin nhắn riêng): " + ex.Message);
                throw;
            }
        }

        public async static Task CapNhatTrangThai(string username, string trangThai)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "UPDATE public.\"TaiKhoan\" SET \"TrangThai\" = @tt, \"ThoiGianHDGanDay\" = @tg WHERE \"TenTK\" = @t";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", username);
                    cmd.Parameters.AddWithValue("tt", trangThai);
                    cmd.Parameters.AddWithValue("tg", DateTime.Now);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(cập nhật trạng thái): " + ex.Message);
            }
        }

        public async static Task<string> TongTK()
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "SELECT COUNT(*) FROM public.\"TaiKhoan\"";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    var reader = await cmd.ExecuteScalarAsync();
                    return reader?.ToString() ?? "";
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tổng tài khoản): " + ex.Message);
            }
            return "1";
        }

        public async static Task<string> TongTN()
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "SELECT COUNT(*) FROM public.\"TinNhan\"";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    var reader = await cmd.ExecuteScalarAsync();
                    return reader?.ToString() ?? "0";
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(tổng tin nhắn): " + ex.Message);
            }
            return "1";
        }

        public async static Task<List<ThongTinTK>> ThongTinTK()
        {
            var list = new List<ThongTinTK>();
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "SELECT \"TenTK\", \"MatKhau\", \"Email\", \"TrangThai\", \"BietDanh\", \"NgayTao\" FROM public.\"TaiKhoan\"";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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
                Console.WriteLine("Lỗi DB(thông tin...): " + ex.Message);
                throw;
            }

            return list;
        }

        public async static Task<List<TinNhanDienDan>> TinNhanDienDan()
        {
            var list = new List<TinNhanDienDan>();
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "SELECT tn.\"TenTK\", tn.\"NoiDung\", tn.\"NgayGui\", tk.\"BietDanh\" FROM public.\"TinNhan\" tn JOIN public.\"TaiKhoan\" tk ON tn.\"TenTK\" = tk.\"TenTK\" WHERE tn.\"MaCTC\" = 0 ORDER BY tn.\"NgayGui\" ASC LIMIT 50";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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

        public async static Task<List<TinNhanRieng>> TinNhanRieng(string user)
        {
            var list = new List<TinNhanRieng>();
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "SELECT ctc.\"MaCTC\", ctc.\"TenCTC\", tn.\"TenTK\", tn.\"NoiDung\", tn.\"NgayGui\" FROM public.\"CuocTroChuyen\" ctc JOIN public.\"ThanhVienNhom\" tvn ON ctc.\"MaCTC\" = tvn.\"MaCTC\" LEFT JOIN public.\"TinNhan\" tn ON ctc.\"MaCTC\" = tn.\"MaCTC\" WHERE tvn.\"TenTK\" = @user AND ctc.\"MaCTC\" <> 0 AND ctc.\"TrangThai\" <> 'Đã bị xóa' ORDER BY tn.\"NgayGui\" ASC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("user", user.Trim());
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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

        public async static Task<int> SoLuongTrucTuyen()
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "SELECT COUNT(*) FROM public.\"TaiKhoan\" WHERE \"TrangThai\" = 'Đang trực tuyến'";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    var reader = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(reader);
                }

            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(số lượng trực tuyến): " + ex.Message);
            }
            return 0;
        }

        public async static Task CamTaiKhoan(string tenTK)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "UPDATE public.\"TaiKhoan\" SET \"TrangThai\" = 'Đã bị cấm' WHERE \"TenTK\" = @t";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", tenTK);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(cấm tài khoản): " + ex.Message);
            }
        }

        public async static Task<int> TaoCTC(string tenCTC)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql1 = "INSERT INTO public.\"CuocTroChuyen\" (\"TenCTC\", \"NgayTao\", \"TrangThai\") VALUES (@t, @n, @r) RETURNING \"MaCTC\"";
                using (var cmd = new NpgsqlCommand(sql1, conn))
                {
                    cmd.Parameters.AddWithValue("t", tenCTC);
                    cmd.Parameters.AddWithValue("n", DateTime.Now);
                    cmd.Parameters.AddWithValue("r", "Bình thường");
                    var reader = await cmd.ExecuteScalarAsync();
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

        public async static Task SuaCTC(int maCTC, string tenCTC)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "update public.\"CuocTroChuyen\" set \"TenCTC\" = @tenCTC where \"MaCTC\" = @maCTC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("tenCTC", tenCTC);
                    cmd.Parameters.AddWithValue("maCTC", maCTC);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(sửa cuộc trò chuyện): " + ex.Message);
            }
        }

        public async static Task XoaCTC(int maCTC)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);

                await conn.OpenAsync();
                string sql = "update public.\"CuocTroChuyen\" set \"TrangThai\" = 'Đã bị xóa' where \"MaCTC\" = @maCTC";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("maCTC", maCTC);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(xóa cuộc trò chuyện): " + ex.Message);
            }
        }

        public async static Task ThemThanhVien(int maCTC, string tenTK)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "INSERT INTO public.\"ThanhVienNhom\" (\"MaCTC\", \"MaTK\", \"TenTK\", \"NgayTG\") VALUES (@m, @ma, @t, @n)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("m", maCTC);
                    cmd.Parameters.AddWithValue("ma", await LayMaTK(tenTK));
                    cmd.Parameters.AddWithValue("t", tenTK.Trim());
                    cmd.Parameters.AddWithValue("n", DateTime.Now);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(thêm thành viên): " + ex.Message);
            }
        }

        public async static Task<int> LayMaTK(string tenTK)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "SELECT \"MaTK\" FROM public.\"TaiKhoan\" WHERE \"TenTK\" = @t ";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t", tenTK.Trim());
                    var reader = await cmd.ExecuteScalarAsync();
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

        public async static Task<bool> KiemTraKetBan(int maTK1, int maTK2)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "select 1 from public.\"BanBe\" where ((\"MaNgGui\" = @m1 AND \"MaNgNhan\" = @m2) OR (\"MaNgGui\" = @m2 AND \"MaNgNhan\" = @m1)) AND \"TrangThai\" = 'Đang chờ' LIMIT 1;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("m1", maTK1);
                    cmd.Parameters.AddWithValue("m2", maTK2);
                    var a = await cmd.ExecuteScalarAsync();
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

        public async static Task KetBan(int maNgGui, int maNgNhan)
        {
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "INSERT INTO public.\"BanBe\" (\"MaNgGui\", \"MaNgNhan\", \"TrangThai\", \"NgayTG\") VALUES (@t1, @t2, @t, @n)";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("t1", maNgGui);
                    cmd.Parameters.AddWithValue("t2", maNgNhan);
                    cmd.Parameters.AddWithValue("t", "Đang chờ");
                    cmd.Parameters.AddWithValue("n", DateTime.Now);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine("Lỗi DB(kết bạn): " + ex.Message);
            }
        }

        public async static Task<List<ThongTinBanBe>> LayDanhSachBanBe(int maTK)
        {
            var bb = new List<ThongTinBanBe>();
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = @"SELECT tk.""MaTK"", tk.""TenTK"", tk.""BietDanh"", bb.""TrangThai""
                                 FROM public.""BanBe"" bb JOIN public.""TaiKhoan"" tk
                                 ON ((bb.""MaNgGui"" = @maTK AND bb.""MaNgNhan"" = tk.""MaTK"") OR
                                     (bb.""MaNgNhan"" = @maTK AND bb.""MaNgGui"" = tk.""MaTK""))
                                 WHERE bb.""TrangThai"" = 'Kết bạn thành công'";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("maTK", maTK);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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

        public async static Task<List<string>> LayLoiMoiKetBan(int maTK)
        {
            var loiMoi = new List<string>();
            try
            {
                using var conn = new NpgsqlConnection(str);
                await conn.OpenAsync();
                string sql = "SELECT tk.\"TenTK\" FROM public.\"BanBe\" bb JOIN public.\"TaiKhoan\" tk ON bb.\"MaNgGui\" = tk.\"MaTK\" WHERE bb.\"MaNgNhan\" = @maTK AND bb.\"TrangThai\" = 'Đang chờ'";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("maTK", maTK);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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
