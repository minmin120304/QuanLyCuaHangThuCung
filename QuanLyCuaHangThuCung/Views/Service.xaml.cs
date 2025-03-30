using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyCuaHangThuCung.Views
{
    /// <summary>
    /// Interaction logic for Service.xaml
    /// </summary>
    public partial class Service : UserControl
    {
        AppDbContext db = new AppDbContext();
        public Service()
        {
            InitializeComponent();
            LoadData();
        }
        void LoadData()
        {
            ServiceTable.ItemsSource = db.Service.ToList();
        }
        private bool validateInput()
        {
            if (string.IsNullOrEmpty(tenDV_input.Text))
            {
                MessageBox.Show("Tên dịch vụ không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(note_input.Text))
            {
                MessageBox.Show("Ghi chú không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(ListType.Text))
            {
                MessageBox.Show("Loại hình dịch vụ không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(gia_input.Text))
            {
                MessageBox.Show("Giá không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!decimal.TryParse(gia_input.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Giá dịch vụ phải là số thực không âm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        void resetInput()
        {
            tenDV_input.Text = "";
            note_input.Text = "";
            gia_input.Text = "";

            ListType.SelectedIndex = -1;
            ServiceTable.SelectedIndex = -1;
            ServiceTable.SelectedItem = null;
            ServiceTable.ItemsSource = db.Service.ToList();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!validateInput()) return;

            if (!decimal.TryParse(gia_input.Text, out decimal price))
            {
                MessageBox.Show("Đơn giá phải là số thực hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Models.Service service = new Models.Service()
            {
                serviceName = tenDV_input.Text,
                note = note_input.Text,
                price = price
            };

            if (ListType.SelectedItem != null)
            {
                if (ListType.SelectedItem is ComboBoxItem item1)
                {
                    service.serviceType = item1.Content.ToString();
                }
                else
                {
                    service.serviceType = ListType.SelectedItem?.ToString() ?? "";
                }
            }

            db.Service.Add(service);
            db.SaveChanges();
            MessageBox.Show("Thêm dịch vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!validateInput()) return;

            string serviceId = ((Models.Service)ServiceTable.SelectedItem).Id;
            Models.Service service = db.Service.FirstOrDefault(p => p.Id == serviceId);

            if (service == null) return;

            service.serviceName = tenDV_input.Text;
            service.note = note_input.Text;
            if (ListType.SelectedItem != null)
            {
                if (ListType.SelectedItem is ComboBoxItem item1)
                {
                    service.serviceType = item1.Content.ToString();
                }
                else
                {
                    service.serviceType = ListType.SelectedItem?.ToString() ?? "";
                }
            }
            service.price = decimal.Parse(gia_input.Text);

            db.SaveChanges();
            MessageBox.Show("Cập nhật dịch vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ServiceTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn dịch vụ để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa dịch vụ này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            db.Service.Remove((Models.Service)ServiceTable.SelectedItem);
            db.SaveChanges();
            MessageBox.Show("Xóa dịch vụ thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ServiceTable.SelectedItem is Models.Service service)
            {
                tenDV_input.Text = service.serviceName;
                note_input.Text = service.note;
                gia_input.Text = service.price.ToString();
                foreach (ComboBoxItem item in ListType.Items)
                {
                    if ((string)item.Content == service.serviceType)
                    {
                        ListType.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim(); // Lấy nội dung từ ô tìm kiếm

            if (!string.IsNullOrEmpty(searchText))
            {
                var filteredService = db.Service
                    .Where(s => s.serviceName.Contains(searchText)
                             || s.serviceType.Contains(searchText))
                    .Select(s => new
                    {
                        Id = s.Id,
                        price = s.price,
                        serviceName = s.serviceName,
                        note = s.note,
                        serviceType = s.serviceType
                    }).ToList();

                ServiceTable.ItemsSource = filteredService;
            }
            else
            {
                LoadData();
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            resetInput();
            LoadData();
        }
    }
}
