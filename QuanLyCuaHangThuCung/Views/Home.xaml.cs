using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
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
            LoadRecentOrders();
            LoadTopSellingProducts();
            LoadTopUsedServices();
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }
        private void LoadRecentOrders()
        {
            var recentOrders = _context.Bill
                .Include(b => b.Customer)
                .Include(b => b.BillDetails)
                .OrderByDescending(b => b.CreatedDate)
                .ToList()
                .Select(b => new
                {
                    Name = b.Customer != null ? b.Customer.customerName : "Khách vãng lai",
                    Details = $"SL: {b.BillDetails.Count()}, Ngày: {b.CreatedDate:dd/MM/yyyy}", 
                    Price = $"{b.TotalAmount:N0} VND" 
                })
                .ToList();

            foreach (var order in recentOrders)
            {
                System.Diagnostics.Debug.WriteLine($"Order: {order.Name} - {order.Details} - {order.Price}");
            }
            donHang.ItemsSource = recentOrders;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm bán chạy nhất
        /// </summary>
        private void LoadTopSellingProducts()
        {
            var topProducts = _context.BillDetail
                .Include(bd => bd.Product)
                .GroupBy(bd => new { bd.Product.Id, bd.Product.productName })
                .Select(g => new
                {
                    Name = g.Key.productName,
                    TotalSold = g.Sum(bd => bd.Quantity) 
                })
                .OrderByDescending(p => p.TotalSold)
                .Take(3)
                .AsEnumerable() 
                .Select(p => new
                {
                    Name = p.Name,
                    Details = $"Đã bán: {p.TotalSold}" 
                })
                .ToList();
            topSanPhamBanChay.Items.Clear();
            topSanPhamBanChay.ItemsSource = topProducts;

        }

        /// <summary>
        /// Lấy danh sách đồ dùng phổ biến
        /// </summary>
        private void LoadTopUsedServices()
        {
            var topServices = _context.BillDetail
                .Include(bd => bd.Service)
                .Where(bd => bd.Service != null)
                .GroupBy(bd => new { bd.Service.Id, bd.Service.serviceName })
                .Select(g => new
                {
                    Name = g.Key.serviceName,
                    UsageCount = g.Count()
                })
                .OrderByDescending(p => p.UsageCount)
                .Take(3)
                .AsEnumerable() 
                .Select(p => new
                {
                    Name = p.Name,
                    Details = $"Lượt dùng: {p.UsageCount}" 
                })
                .ToList();

            topDichVuPhoBien.Items.Clear();
            topDichVuPhoBien.ItemsSource = topServices;
        }
    }
}
