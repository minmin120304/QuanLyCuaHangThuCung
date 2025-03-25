using QuanLyCuaHangThuCung.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuanLyCuaHangThuCung.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Account : UserControl
    {
        AppDbContext Db = new AppDbContext();
        private User _CurUser;
        public Account()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            string userID = CurrentSession.Instance.UserId;
            _CurUser = Db.User.FirstOrDefault(u => u.Username == userID);

            if (_CurUser != null) { 
                name_input.Text = _CurUser.FullName;
                email_input.Text = _CurUser.Email;
                phoneNum_input.Text = _CurUser.phoneNum;
                address_input.Document.Blocks.Clear();
                address_input.Document.Blocks.Add(new Paragraph(new Run(_CurUser.Address)));
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(address_input.Document.ContentStart, address_input.Document.ContentEnd);

            if (_CurUser != null)
            {
                _CurUser.FullName = name_input.Text;
                _CurUser.Email = email_input.Text;
                _CurUser.phoneNum = phoneNum_input.Text;
                _CurUser.Address = textRange.Text;

                Db.SaveChanges();

                // Cập nhật giá trị và UI sẽ tự động thay đổi nhờ INotifyPropertyChanged
                CurrentSession.Instance.FullName = _CurUser.FullName;

                MessageBox.Show("Thông tin đã được cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
