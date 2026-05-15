namespace Server
{
    public class RegisterRequest
    {
        public string TenTK { get; set; } = "";
        public string MatKhau { get; set; } = "";
        public string Email { get; set; } = "";
        public int Sdt { get; set; }
        public string Captcha { get; set; } = "";
    }
}
