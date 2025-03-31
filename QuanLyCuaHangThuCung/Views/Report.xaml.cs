using OxyPlot;
using OxyPlot.Series;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OxyPlot.Axes; // Import models và DbContext

namespace QuanLyCuaHangThuCung.Views
{
    public partial class Report : UserControl
    {
        private readonly AppDbContext Db = new AppDbContext();
        public PlotModel MyPlotModel { get; set; }

        public Report()
        {
            InitializeComponent();
            this.Loaded += Report_Loaded;
        }

        private void Report_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBoxes();
            LoadData();
        }

        private void LoadComboBoxes()
        {
            cbFromYear.IsEnabled = false;
            cbToYear.IsEnabled = false;

            // Lấy danh sách các năm có hóa đơn trong cơ sở dữ liệu
            var years = Db.Bill
                .Select(b => b.CreatedDate.Year)
                .Distinct()
                .OrderBy(y => y)
                .Select(y => y.ToString())
                .ToList();

            if (years.Any())
            {
                cbYear.ItemsSource = years;
                cbFromYear.ItemsSource = years;
                cbToYear.ItemsSource = years;

                cbYear.SelectedIndex = 0;
                cbFromYear.SelectedIndex = 0;
                cbToYear.SelectedIndex = years.Count - 1;
            }
        }


        private void LoadData()
        {
            dpFromDate.SelectedDate = DateTime.Now.AddMonths(-1); // 1 tháng trước
            dpToDate.SelectedDate = DateTime.Now; // Hiện tại

            LoadWarehouseStatistics(dpFromDate.SelectedDate.Value, dpToDate.SelectedDate.Value);
            LoadLoyalCustomers();
            LoadRevenueChart("month", DateTime.Now.Year);
        }

        private void LoadWarehouseStatistics(DateTime fromDate, DateTime toDate)
        {
            var warehouseData = Db.Product
                .Select(k => new
                {
                    k.productName,

                    // Tổng số lượng nhập trong khoảng thời gian
                    TongSoLuong = Db.Product
                        .Where(p => p.Id == k.Id && p.DateInput >= fromDate && p.DateInput <= toDate)
                        .Sum(p => (int?)p.quantity) ?? 0,

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
                .ToList();

            dataGridLoyalCustomers.ItemsSource = loyalCustomers;
        }
        private void LoadRevenueChart(string type, int year, int month = 0, int fromYear = 0, int toYear = 0)
        {
            MyPlotModel = new PlotModel { Title = "Doanh Thu" };

            // Trục Y là CategoryAxis (cần thiết cho BarSeries)
            var categoryAxis = new CategoryAxis { Position = AxisPosition.Left, Title = "Thời gian", AxisTitleDistance = 8 };

            // Trục X là LinearAxis (để hiển thị doanh thu theo giá trị số)
            var valueAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Doanh thu (VNĐ)",
                StringFormat = "N0"
            };

            var barSeries = new BarSeries { Title = "VNĐ", FillColor = OxyColors.Blue };

            switch (type)
            {
                case "month":
                    for (int i = 1; i <= 12; i++)
                    {
                        categoryAxis.Labels.Add($"Tháng {i}");
                        barSeries.Items.Add(new BarItem { Value = (double)GetRevenueByMonth(i, year) });
                    }
                    break;
                case "yearRange":
                    for (int i = fromYear; i <= toYear; i++)
                    {
                        categoryAxis.Labels.Add(i.ToString());
                        barSeries.Items.Add(new BarItem { Value = (double)GetRevenueByYear(i) });
                    }
                    break;
            }

            // Thêm trục vào biểu đồ
            MyPlotModel.Axes.Add(categoryAxis);
            MyPlotModel.Axes.Add(valueAxis);
            MyPlotModel.Series.Add(barSeries);

            // Cập nhật biểu đồ hiển thị
            plotView.Model = MyPlotModel;
        }

        private decimal GetRevenueByYear(int year) => Db.Bill.Where(hd => hd.CreatedDate.Year == year).Sum(hd => (decimal?)hd.TotalAmount) ?? 0;

        private decimal GetRevenueByMonth(int month, int year) => Db.Bill.Where(hd => hd.CreatedDate.Month == month && hd.CreatedDate.Year == year).Sum(hd => (decimal?)hd.TotalAmount) ?? 0;

        private void BtnStatistic_Click(object sender, RoutedEventArgs e)
        {
            if (cbFilterType.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedType = selectedItem.Tag.ToString();
                
                if (selectedType == "month")
                {
                    int year = int.Parse(cbYear.SelectedItem.ToString());
                    LoadRevenueChart("month", year);
                }
                else if (selectedType == "year")
                {
                    int fromYear = int.Parse(cbFromYear.SelectedItem.ToString());
                    int toYear = int.Parse(cbToYear.SelectedItem.ToString());
                    LoadRevenueChart("yearRange", 0, 0, fromYear, toYear);
                }
            }
        }
        private void CbFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFilterType.SelectedItem is ComboBoxItem selectedItem)
            {
                if (!IsLoaded) return;
                string selectedType = selectedItem.Tag.ToString(); // Lấy giá trị từ Tag

                if (selectedType == "month")
                {
                    if (cbYear != null)
                    {
                        cbYear.IsEnabled = true;
                        cbYear.Visibility = Visibility.Visible;
                    }
                    cbFromYear.Visibility = Visibility.Collapsed;
                    cbFromYear.IsEnabled = false;
                    cbToYear.Visibility = Visibility.Collapsed;
                    cbToYear.IsEnabled = false;
                }
                else if (selectedType == "year")
                {
                    if (cbYear != null)
                    {
                        cbYear.IsEnabled = false;
                        cbYear.Visibility = Visibility.Collapsed;
                    }
                    cbFromYear.Visibility = Visibility.Visible;
                    cbFromYear.IsEnabled = true;
                    cbToYear.Visibility = Visibility.Visible;
                    cbToYear.IsEnabled = true;
                }
            }
        }
        private void Find_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = ""; // Xóa ô nhập tìm kiếm
            LoadData(); // Gọi lại hàm load dữ liệu ban đầu
            MessageBox.Show("Dữ liệu đã được làm mới!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra ngày hợp lệ
            if (dpFromDate.SelectedDate == null || dpToDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime fromDate = dpFromDate.SelectedDate.Value;
            DateTime toDate = dpToDate.SelectedDate.Value;

            if (fromDate > toDate)
            {
                MessageBox.Show("Ngày bắt đầu không thể lớn hơn ngày kết thúc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Cập nhật thống kê kho
            LoadWarehouseStatistics(fromDate, toDate);
        }
    }
}
