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
    /// Interaction logic for SanPham.xaml
    /// </summary>
    public partial class SanPham : UserControl
    {
        AppDbContext db = new AppDbContext();
        public SanPham()
        {
            InitializeComponent();
            LoadData();
        }
        void LoadData()
        {
            ProductTable.ItemsSource = db.Product.ToList();
        }
        private bool validateInput()
        {
            if (string.IsNullOrEmpty(tenSP_input.Text))
            {
                MessageBox.Show("Tên sản phẩm không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(xuatXu_input.Text))
            {
                MessageBox.Show("Xuất xứ không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(SL_input.Text))
            {
                MessageBox.Show("Số lượng không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(ListDV.Text))
            {
                MessageBox.Show("Đơn vị không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(ListTypeSP.Text))
            {
                MessageBox.Show("Loại sản phẩm không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (string.IsNullOrEmpty(gia_input.Text))
            {
                MessageBox.Show("Giá không được để trống", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!int.TryParse(SL_input.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Số lượng phải là số nguyên không âm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!decimal.TryParse(gia_input.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Giá sản phẩm phải là số thực không âm.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        void resetInput()
        {
            tenSP_input.Text = "";
            xuatXu_input.Text = "";
            SL_input.Text = "";
            gia_input.Text = "";

            ListDV.SelectedIndex = -1;
            ListTypeSP.SelectedIndex = -1;
            ProductTable.SelectedIndex = -1;
            ProductTable.SelectedItem = null;
            ProductTable.ItemsSource = db.Product.ToList();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!validateInput()) return;

            if (!int.TryParse(SL_input.Text, out int quantity))
            {
                MessageBox.Show("Số lượng phải là số nguyên hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(gia_input.Text, out decimal price))
            {
                MessageBox.Show("Đơn giá phải là số thực hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Models.Product product = new Models.Product()
            {
                productName = tenSP_input.Text,
                origin = xuatXu_input.Text,
                quantity = quantity,
                price = price
            };

            if (ListDV.SelectedItem != null || ListTypeSP.SelectedItem != null)
            {
                // Kiểm tra xem SelectedItem có phải là ComboBoxItem không
                if (ListDV.SelectedItem is ComboBoxItem item)
                {
                    product.unit = item.Content.ToString();
                }
                else
                {
                    product.unit = ListDV.SelectedItem?.ToString() ?? "";
                }
                if (ListTypeSP.SelectedItem is ComboBoxItem item1)
                {
                    product.type = item1.Content.ToString();
                }
                else
                {
                    product.type = ListTypeSP.SelectedItem?.ToString()?? "";
                }
            }

            db.Product.Add(product);
            db.SaveChanges();
            MessageBox.Show("Thêm sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (ProductTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để sửa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!validateInput()) return;

            string productId = ((Models.Product)ProductTable.SelectedItem).Id;
            Models.Product product = db.Product.FirstOrDefault(p => p.Id == productId);

            if (product == null) return;

            product.productName = tenSP_input.Text;
            product.origin = xuatXu_input.Text;
            if (ListDV.SelectedItem != null || ListTypeSP.SelectedItem != null)
            {
                // Kiểm tra xem SelectedItem có phải là ComboBoxItem không
                if (ListDV.SelectedItem is ComboBoxItem item)
                {
                    product.unit = item.Content.ToString();
                }
                else
                {
                    product.unit = ListDV.SelectedItem?.ToString() ?? "";
                }
                if (ListTypeSP.SelectedItem is ComboBoxItem item1)
                {
                    product.type = item1.Content.ToString();
                }
                else
                {
                    product.type = ListTypeSP.SelectedItem?.ToString() ?? "";
                }
            }
            product.quantity = int.Parse(SL_input.Text);
            product.price = decimal.Parse(gia_input.Text);

            db.SaveChanges();
            MessageBox.Show("Cập nhật sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ProductTable.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn sản phẩm để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            db.Product.Remove((Models.Product)ProductTable.SelectedItem);
            db.SaveChanges();
            MessageBox.Show("Xóa sản phẩm thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            resetInput();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if (ProductTable.SelectedItem is Models.Product product)
           {
                tenSP_input.Text = product.productName;
                xuatXu_input.Text = product.origin;
                SL_input.Text = product.quantity.ToString();
                gia_input.Text = product.price.ToString();
                foreach (ComboBoxItem item in ListDV.Items)
                {
                    if ((string)item.Content == product.unit)
                    {
                        ListDV.SelectedItem = item;
                        break;
                    }
                }
                foreach (ComboBoxItem item in ListTypeSP.Items)
                {
                    if ((string)item.Content == product.type)
                    {
                        ListTypeSP.SelectedItem = item;
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
                var filteredProduct = db.Product
                    .Where( p => p.productName.Contains(searchText)
                             || p.type.Contains(searchText) 
                             || p.origin.Contains(searchText)
                             || p.unit.Contains(searchText))
                    .Select(p => new
                    {
                        Id = p.Id,
                        productName = p.productName,
                        origin = p.origin,
                        type = p.type,
                        quantity = p.quantity,
                        price = p.price,
                        unit = p.unit
                    }).ToList();

                ProductTable.ItemsSource = filteredProduct;
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
