using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using QuanLyCuaHangThuCung.Models;
using QuanLyCuaHangThuCung.Utils;

namespace QuanLyCuaHangThuCung.Views
{
    public partial class SignUp : Window
    {
        private AppDbContext Db = new AppDbContext();

        public SignUp()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text;
            string email = txtEmail.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Db.User.Any(u => u.Username == username))
            {
                MessageBox.Show("Tài khoản đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (Db.User.Any(u => u.Email == email))
            {
                MessageBox.Show("Email đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Username = username,
                PasswordHash = PasswordHelper.HashPassword(password) // Mã hóa mật khẩu
            };

            Db.User.Add(user);
            Db.SaveChanges();

            MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            // Mở lại Form đăng nhập
            Login loginForm = new Login();
            loginForm.Show();

            // Đóng Form hiện tại
            this.Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Close();
        }
    }
}
