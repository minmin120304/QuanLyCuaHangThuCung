using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using QuanLyCuaHangThuCung.Models;
using QuanLyCuaHangThuCung.Utils; // Thêm class hỗ trợ mã hóa

namespace QuanLyCuaHangThuCung.Views
{
    public partial class Login : Window
    {
        private AppDbContext Db = new AppDbContext();

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = account_input.Text;
            string password = pass_input.Password;

            var user = Db.User.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                MessageBox.Show("Tài khoản không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra mật khẩu (đã mã hóa)
            if (PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Mở màn hình chính
                MainWindow main = new MainWindow();
                main.Show();

                this.Close(); // Đóng cửa sổ đăng nhập

            }
            else
            {
                MessageBox.Show("Sai mật khẩu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            SignUp signup = new SignUp();
            signup.Show();
            this.Close();
        }

        private void HienThiPassCheck_Checked(object sender, RoutedEventArgs e)
        {
            pass_input_visible.Text = pass_input.Password;
            pass_input_visible.Visibility = Visibility.Visible;
            pass_input.Visibility = Visibility.Collapsed;
        }

        private void HienThiPassCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            pass_input.Password = pass_input_visible.Text;
            pass_input_visible.Visibility = Visibility.Collapsed;
            pass_input.Visibility = Visibility.Visible;
        }

    }
}
