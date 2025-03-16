using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls; // Thay thế bằng context của bạn

namespace QuanLyCuaHangThuCung.Views
{
    public partial class Report : UserControl
    {
        private AppDbContext Db = new AppDbContext(); // Đảm bảo có DbContext

        public Report()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var data = GetRevenueByMonth(2024);
            DoanhThuTable.ItemsSource = data;
        }

        private List<RevenueReport> GetRevenueByMonth(int year)
        {
            var result = Db.Bill
                .Where(b => b.CreatedDate.Year == year)
                .GroupBy(b => b.CreatedDate.Month)
                .Select(g => new RevenueReport
                {
                    Month = g.Key,
                    TotalRevenue = g.Sum(b => (decimal)b.TotalAmount)
                })
                .OrderBy(r => r.Month)
                .ToList();

            return result;
        }

        private void DoanhThu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }
    }

    // Model để chứa dữ liệu báo cáo doanh thu
    public class RevenueReport
    {
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
    }

}
