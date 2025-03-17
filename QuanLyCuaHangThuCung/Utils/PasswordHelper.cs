using System;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyCuaHangThuCung.Utils
{
    public static class PasswordHelper
    {
        // Mã hóa mật khẩu bằng SHA-256
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Kiểm tra mật khẩu nhập vào có khớp với mật khẩu trong DB không
        public static bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }
    }
}
