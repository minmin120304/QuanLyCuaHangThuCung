using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Controls;
using QuanLyCuaHangThuCung.Models;

namespace QuanLyCuaHangThuCung.Views
{
    public partial class Home : UserControl
    {
        private AppDbContext _context;

        public Home()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                // Lấy tổng doanh thu từ bảng Bill
                var totalRevenue = _context.Bill.Sum(b => (decimal?)b.TotalAmount) ?? 0;
                doanhThu.Content = $"{totalRevenue:N0} VND"; // Định dạng số tiền có dấu chấm

                // Số lượng sản phẩm hiện có trong kho
                int totalProducts = _context.Product.Sum(p => p.quantity);
                SLsp.Content = totalProducts;

                // Số loại dịch vụ
                int totalServices = _context.Service.Count();
                slDV.Content = totalServices;

                // Lấy danh sách sản phẩm "Thức ăn" (Food) và tổng số lượng đã bán
                //var foodProducts = _context.Product
                //    .Where(p => p.type.Trim() == "Thức ăn")
                //    .Select(p => new
                //    {
                //        p.productName,
                //        SoldQuantity = _context.BillDetail
                //            .Where(bd => bd.ProductId == p.Id)
                //            .Sum(bd => (int?)bd.Quantity) ?? 0 // Tránh lỗi null
                //    })
                //    .ToList();
                //FoodTable.ItemsSource = foodProducts;

                // Lấy danh sách sản phẩm "Phụ kiện" (Accessories) và tổng số lượng đã bán
                //var accessoryProducts = _context.Product
                //    .Where(p => p.type.Trim() == "Phụ kiện")
                //    .Select(p => new
                //    {
                //        p.productName,
                //        SoldQuantity = _context.BillDetail
                //            .Where(bd => bd.ProductId == p.Id)
                //            .Sum(bd => (int?)bd.Quantity) ?? 0
                //    })
                //    .ToList();
                //DoDungTable.ItemsSource = accessoryProducts;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }
    }
}
