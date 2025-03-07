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
    /// Interaction logic for Employee.xaml
    /// </summary>
    public partial class Employee : UserControl
    {
        AppDbContext db = new AppDbContext();
        public Employee()
        {
            InitializeComponent();
            EmployeeTable.ItemsSource = db.Employee.ToList();
        }
        private bool validateInput()
        {
            if (string.IsNullOrEmpty(tenNV_input.Text))
            {
                MessageBox.Show("Tên nhân viên không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

            tenNV_input.Text = "";
            textRange.Text = "";
            SDT_input.Text = "";
            
            EmployeeTable.SelectedIndex = -1;
            EmployeeTable.SelectedItem = null;
            EmployeeTable.ItemsSource = db.Employee.ToList();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!validateInput()) return;
            TextRange textRange = new TextRange(diaChi_input.Document.ContentStart, diaChi_input.Document.ContentEnd);

            Models.Employee employee = new Models.Employee()
            {
                employeeName = tenNV_input.Text,
                address = textRange.Text,
                phoneNum = SDT_input.Text
            };

            db.Employee.Add(employee);
            db.SaveChanges();
            MessageBox.Show("Thêm nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!validateInput()) return;

            string employeeID = ((Models.Employee)EmployeeTable.SelectedItem).Id;
            Models.Employee employee = db.Employee.FirstOrDefault(c => c.Id == employeeID);
  
            if (employee == null) return;

            TextRange textRange = new TextRange(diaChi_input.Document.ContentStart, diaChi_input.Document.ContentEnd);
            employee.employeeName = tenNV_input.Text;
            employee.address = textRange.Text;
            employee.phoneNum = SDT_input.Text;

            db.SaveChanges();
            MessageBox.Show("Cập nhật nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn nhân viên để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            db.Employee.Remove((Models.Employee)EmployeeTable.SelectedItem);
            db.SaveChanges();
            MessageBox.Show("Xóa nhân viên thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeTable.SelectedItem is Models.Employee employee)
            {
                TextRange textRange = new TextRange(diaChi_input.Document.ContentStart, diaChi_input.Document.ContentEnd);

                tenNV_input.Text = employee.employeeName;
                textRange.Text = employee.address;
                SDT_input.Text = employee.phoneNum;
            }
        }

        private void ProductTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString(); // STT bắt đầu từ 1
        }

    }
}
