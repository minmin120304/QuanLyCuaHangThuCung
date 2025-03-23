using OxyPlot;
using OxyPlot.Series;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyCuaHangThuCung.Models; // Import models và DbContext

namespace QuanLyCuaHangThuCung.Views
{
    public partial class Report : UserControl
    {
        private readonly AppDbContext Db = new AppDbContext();
        public PlotModel MyPlotModel { get; set; }

        public Report()
        {
            InitializeComponent();
            // Kiểm tra cbMonth
            if (cbMonth == null)
            {
                MessageBox.Show("cbMonth chưa được khởi tạo!");
            }
            cbMonth.IsEnabled = false;
            cbYear.IsEnabled = false;
            LoadComboBoxes();
            LoadData();
        }

        private void LoadComboBoxes()
        {
            cbMonth.ItemsSource = Enumerable.Range(1, 12);
            cbMonth.SelectedIndex = 0;

            int currentYear = DateTime.Now.Year;
            cbYear.ItemsSource = Enumerable.Range(2020, currentYear - 2019);
            cbYear.SelectedItem = currentYear;
        }

        private void LoadData()
        {
            DateTime fromDate = DateTime.Now.AddMonths(-1); // 1 tháng trước
            DateTime toDate = DateTime.Now; // Hiện tại

            LoadWarehouseStatistics(fromDate, toDate);
            LoadInvoiceStatistics();
            LoadLoyalCustomers();
            LoadRevenueChart();
        }

        private void LoadWarehouseStatistics(DateTime fromDate, DateTime toDate)
        {
            var warehouseData = Db.Product
                .Select(k => new
                {
                    k.productName,

                    // Tổng số lượng nhập trong khoảng thời gian
                    TongSoLuong = Db.Product
                        .Where(nh => nh.Id == k.Id && nh.DateInput >= fromDate && nh.DateInput <= toDate)
                        .Sum(nh => (int?)nh.quantity) ?? 0,

                    // Tổng số lượng đã bán trong khoảng thời gian
                    SoLuongDaBan = Db.BillDetail
                        .Where(hd => hd.ProductId == k.Id && hd.Bill.CreatedDate >= fromDate && hd.Bill.CreatedDate <= toDate)
                        .Sum(hd => (int?)hd.Quantity) ?? 0
                })
                .ToList()
                .Select(k => new
                {
                    k.productName,
                    k.TongSoLuong,
                    k.SoLuongDaBan,
                    SoLuongConLai = k.TongSoLuong - k.SoLuongDaBan
                })
                .ToList();

            dataGridWarehouse.ItemsSource = warehouseData;
        }

        private void LoadInvoiceStatistics()
        {
            var invoiceData = Db.Bill
                .Select(hd => new
                {
                    hd.Id,
                    hd.CreatedDate,
                    Customer = Db.Customer.FirstOrDefault(c => c.Id == hd.CustomerId).customerName,
                    hd.TotalAmount,
                    hd.Note
                }).ToList();

            dataGridInvoices.ItemsSource = invoiceData;
        }

        private void LoadLoyalCustomers()
        {
            var loyalCustomers = Db.Customer
                .Select(kh => new
                {
                    kh.Id,
                    kh.customerName,
                    SoLuongDonHang = Db.Bill.Count(hd => hd.CustomerId == kh.Id),
                    SoTien = Db.Bill.Where(hd => hd.CustomerId == kh.Id).Sum(hd => (decimal?)hd.TotalAmount) ?? 0
                })
                .OrderByDescending(kh => kh.SoLuongDonHang)
                .Take(10) // Lấy top 10 khách hàng
                .ToList();

            dataGridLoyalCustomers.ItemsSource = loyalCustomers;
        }

        private void LoadRevenueChart()
        {
            MyPlotModel = new PlotModel { Title = "Doanh Thu Theo Tháng" };
            var barSeries = new BarSeries { Title = "VNĐ" };

            for (int i = 1; i <= 12; i++)
            {
                barSeries.Items.Add(new BarItem { Value = (double)GetRevenueByMonth(i, DateTime.Now.Year) });
            }

            MyPlotModel.Series.Add(barSeries);
            plotView.Model = MyPlotModel;
        }

        private decimal GetRevenueByMonth(int month, int year)
        {
            return Db.Bill
                .Where(hd => hd.CreatedDate.Month == month && hd.CreatedDate.Year == year)
                .Sum(hd => (decimal?)hd.TotalAmount) ?? 0;
        }

        private void BtnStatistic_Click(object sender, RoutedEventArgs e)
        {
            if (cbMonth.SelectedItem == null || cbYear.SelectedItem == null) return;

            int selectedMonth = (int)cbMonth.SelectedItem;
            int selectedYear = (int)cbYear.SelectedItem;

            MyPlotModel = new PlotModel { Title = $"Doanh Thu {selectedMonth}/{selectedYear}" };
            var barSeries = new BarSeries { Title = "VNĐ" };

            barSeries.Items.Add(new BarItem { Value = (double)GetRevenueByMonth(selectedMonth, selectedYear) });

            MyPlotModel.Series.Add(barSeries);
            plotView.Model = MyPlotModel;
        }

        private void CbFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsLoaded) return; // Đảm bảo UI đã tải xong trước khi chạy
            if (cbMonth != null)
            {
                cbMonth.IsEnabled = cbFilterType.SelectedIndex == 0;
            }

        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
