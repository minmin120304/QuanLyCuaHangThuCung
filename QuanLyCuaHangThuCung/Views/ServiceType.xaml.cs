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
    /// Interaction logic for ServiceType.xaml
    /// </summary>
    public partial class ServiceType : UserControl
    {
        AppDbContext db = new AppDbContext();
        public ServiceType()
        {
            InitializeComponent();
            ServiceTypeTable.ItemsSource = db.ServiceType.ToList();
        }
        private bool validateInput()
        {
            if (string.IsNullOrEmpty(maLH_input.Text))
            {
                MessageBox.Show("Mã loại hình không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(tenLH_input.Text))
            {
                MessageBox.Show("Tên loại hình không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        void resetInput()
        {
            maLH_input.Text = "";
            tenLH_input.Text = "";
            note_input.Text = "";

            ServiceTypeTable.SelectedIndex = -1;
            ServiceTypeTable.SelectedItem = null;
            ServiceTypeTable.ItemsSource = db.ServiceType.ToList();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!validateInput()) return;

            Models.ServiceType serviceType = new Models.ServiceType()
            {
                typeID = maLH_input.Text,
                typeName = tenLH_input.Text,
                note = note_input.Text
            };

            db.ServiceType.Add(serviceType);
            db.SaveChanges();
            MessageBox.Show("Thêm loại hình dịch vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceTypeTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn loại hình để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!validateInput()) return;

            string serviceTypeId = ((Models.ServiceType)ServiceTypeTable.SelectedItem).Id;
            Models.ServiceType serviceType = db.ServiceType.FirstOrDefault(p => p.Id == serviceTypeId);

            if (serviceType == null) return;

            serviceType.typeID = maLH_input.Text;
            serviceType.typeName = tenLH_input.Text;
            serviceType.note = note_input.Text;

            db.SaveChanges();
            MessageBox.Show("Cập nhật loại hình thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceTypeTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn loại hình để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa loại hình dịch vụ này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            db.ServiceType.Remove((Models.ServiceType)ServiceTypeTable.SelectedItem);
            db.SaveChanges();
            MessageBox.Show("Xóa loại hình thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if (ServiceTypeTable.SelectedItem is Models.ServiceType serviceType)
           {
                maLH_input.Text = serviceType.typeID;
                tenLH_input.Text = serviceType.typeName;
                note_input.Text = serviceType.note;
            }
        }

        private void ServiceTypeTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString(); // STT bắt đầu từ 1
        }

        private void ServiceTypeTable_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
