using QuanLyCuaHangThuCung.CSDL;
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
        ProductList db = new ProductList();
        public SanPham()
        {
            InitializeComponent();
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
            if (!double.TryParse(gia_input.Text, out double price) || price < 0)
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

            if (!double.TryParse(gia_input.Text, out double price))
            {
                MessageBox.Show("Đơn giá phải là số thực hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Models.Product product = new Models.Product()
            {
                productName = tenSP_input.Text,
                origin = xuatXu_input.Text,
                unit = ListDV.SelectedIndex.ToString(),
                quantity = quantity,
                price = price
            };

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

            int productId = ((Models.Product)ProductTable.SelectedItem).Id;
            Models.Product product = db.Product.FirstOrDefault(p => p.Id == productId);

            if (product == null) return;

            product.productName = tenSP_input.Text;
            product.origin = xuatXu_input.Text;
            product.unit = ListDV.SelectedItem.ToString();
            product.quantity = int.Parse(SL_input.Text);
            product.price = double.Parse(gia_input.Text);

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
                ListDV.SelectedItem = product.unit;
                gia_input.Text = product.price.ToString();
           }
        }

        private void ProductTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString(); // STT bắt đầu từ 1
        }

        private void ProductTable_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
