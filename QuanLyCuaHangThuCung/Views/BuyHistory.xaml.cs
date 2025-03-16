using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuanLyCuaHangThuCung.Models;

namespace QuanLyCuaHangThuCung.Views
{
    public partial class BuyHistory : Window
    {
        private string customerId; // ID của khách hàng được chọn
        private AppDbContext Db = new AppDbContext();

        public BuyHistory(string selectedCustomerId)
        {
            InitializeComponent();
            customerId = selectedCustomerId;
            LoadBuyHistory();
        }

        private void LoadBuyHistory()
        {
            try
            {
                // Lấy danh sách hóa đơn của khách hàng dựa trên customerId
                var bills = Db.Bill
                    .Where(b => b.CustomerId == customerId)
                    .Select(b => new
                    {
                        BillId = b.Id,
                        DateBuy = b.CreatedDate,
                        TotalPrice = b.TotalAmount,
                        Note = b.Note
                    }).ToList();

                if (bills.Any())
                {
                    ListBuy.ItemsSource = bills;
                }
                else
                {
                    MessageBox.Show("Không tìm thấy lịch sử mua hàng cho khách hàng này!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải lịch sử mua hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
