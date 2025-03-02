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
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : UserControl
    {
        AppDbContext db = new AppDbContext();
        public Customer()
        {
            InitializeComponent();
            CustomerTable.ItemsSource = db.Customer.ToList();
        }
        private bool validateInput()
        {
            if (string.IsNullOrEmpty(tenKH_input.Text))
            {
                MessageBox.Show("Tên khách hàng không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            TextRange textRange = new TextRange(diaChi_input.Document.ContentStart, diaChi_input.Document.ContentEnd);
            if (string.IsNullOrWhiteSpace(textRange.Text))
            {
                MessageBox.Show("Địa chỉ không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (string.IsNullOrEmpty(SDT_input.Text))
            {
                MessageBox.Show("Số điện thoại không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        void resetInput()
        {
            TextRange textRange = new TextRange(diaChi_input.Document.ContentStart, diaChi_input.Document.ContentEnd);

            tenKH_input.Text = "";
            textRange.Text = "";
            SDT_input.Text = "";
            
            CustomerTable.SelectedIndex = -1;
            CustomerTable.SelectedItem = null;
            CustomerTable.ItemsSource = db.Customer.ToList();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!validateInput()) return;
            TextRange textRange = new TextRange(diaChi_input.Document.ContentStart, diaChi_input.Document.ContentEnd);

            Models.Customer customer = new Models.Customer()
            {
                customerName = tenKH_input.Text,
                address = textRange.Text,
                phoneNum = SDT_input.Text
            };

            db.Customer.Add(customer);
            db.SaveChanges();
            MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!validateInput()) return;

            int customerID = ((Models.Customer)CustomerTable.SelectedItem).Id;
            Models.Customer customer = db.Customer.FirstOrDefault(c => c.Id == customerID);
  
            if (customer == null) return;

            TextRange textRange = new TextRange(diaChi_input.Document.ContentStart, diaChi_input.Document.ContentEnd);
            customer.customerName = tenKH_input.Text;
            customer.address = textRange.Text;
            customer.phoneNum = SDT_input.Text;

            db.SaveChanges();
            MessageBox.Show("Cập nhật khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn khách hàng để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            db.Customer.Remove((Models.Customer)CustomerTable.SelectedItem);
            db.SaveChanges();
            MessageBox.Show("Xóa khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerTable.SelectedItem is Models.Customer customer)
            {
                TextRange textRange = new TextRange(diaChi_input.Document.ContentStart, diaChi_input.Document.ContentEnd);

                tenKH_input.Text = customer.customerName;
                textRange.Text = customer.address;
                SDT_input.Text = customer.phoneNum;
            }
        }

        private void ProductTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString(); // STT bắt đầu từ 1
        }

    }
}
