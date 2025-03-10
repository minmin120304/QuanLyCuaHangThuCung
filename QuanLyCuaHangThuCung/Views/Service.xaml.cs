using QuanLyCuaHangThuCung.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyCuaHangThuCung.Views
{
    public partial class Service : UserControl
    {
        private AppDbContext _context;
        public ObservableCollection<QuanLyCuaHangThuCung.Models.ServiceType> ListServiceTypes { get; set; }
        public ObservableCollection<QuanLyCuaHangThuCung.Models.Service> Services { get; set; }
        public QuanLyCuaHangThuCung.Models.Service SelectedService { get; set; }

        public Service()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            ListServiceTypes = new ObservableCollection<QuanLyCuaHangThuCung.Models.ServiceType>(
                _context.ServiceType.ToList()
            );

            Services = new ObservableCollection<QuanLyCuaHangThuCung.Models.Service>(
                _context.Service.Include("ServiceType").ToList()
            );
        }
        void resetInput()
        {
            tenDV_input.Text = "";
            note_input.Text = "";
            gia_input.Text = "";

            ListType.SelectedIndex = -1;
            ServiceTable.SelectedIndex = -1;
            ServiceTable.SelectedItem = null;
            ServiceTable.ItemsSource = _context.Service.ToList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenDV_input.Text) || ListType.SelectedValue == null || string.IsNullOrWhiteSpace(gia_input.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(gia_input.Text, out decimal price))
                {
                    MessageBox.Show("Đơn giá phải là số hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var newService = new QuanLyCuaHangThuCung.Models.Service
                {
                    Id = null, // ID sẽ được tự động tạo trong SaveChanges
                    serviceName = tenDV_input.Text,
                    ServiceTypeId = ListType.SelectedValue.ToString(),
                    price = price,
                    note = note_input.Text
                };

                _context.Service.Add(newService);
                _context.SaveChanges();
                Services.Add(newService);

                MessageBox.Show("Thêm dịch vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm dịch vụ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            resetInput();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một dịch vụ để sửa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var serviceToUpdate = (QuanLyCuaHangThuCung.Models.Service)ServiceTable.SelectedItem;
                serviceToUpdate.serviceName = tenDV_input.Text;
                serviceToUpdate.ServiceTypeId = ListType.SelectedValue.ToString();
                serviceToUpdate.price = decimal.Parse(gia_input.Text);
                serviceToUpdate.note = note_input.Text;

                _context.SaveChanges();
                var index = Services.IndexOf(serviceToUpdate);
                if (index >= 0)
                {
                    Services[index] = new QuanLyCuaHangThuCung.Models.Service
                    {
                        Id = serviceToUpdate.Id, // Giữ nguyên ID
                        serviceName = tenDV_input.Text,
                        ServiceTypeId = ListType.SelectedValue.ToString(),
                        price = decimal.Parse(gia_input.Text),
                        note = note_input.Text
                    };
                }

                MessageBox.Show("Cập nhật dịch vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật dịch vụ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            resetInput();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một dịch vụ để xoá.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var serviceToDelete = (QuanLyCuaHangThuCung.Models.Service)ServiceTable.SelectedItem;

            MessageBoxResult result = MessageBox.Show($"Bạn có chắc chắn muốn xoá dịch vụ '{serviceToDelete.serviceName}' không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Service.Remove(serviceToDelete);
                    _context.SaveChanges();

                    // Cập nhật lại danh sách dịch vụ
                    LoadData(); // Đọc lại dữ liệu từ CSDL
                    MessageBox.Show("Xóa dịch vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xoá dịch vụ: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            resetInput();
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceTable.SelectedItem != null)
            {
                SelectedService = (QuanLyCuaHangThuCung.Models.Service)ServiceTable.SelectedItem;
                tenDV_input.Text = SelectedService.serviceName;
                ListType.SelectedValue = SelectedService.ServiceTypeId;
                gia_input.Text = SelectedService.price.ToString();
                note_input.Text = SelectedService.note;
            }
        }
    }
}
