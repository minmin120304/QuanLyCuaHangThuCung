using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace QuanLyCuaHangThuCung.Views
{
    /// <summary>
    /// Interaction logic for Bill.xaml
    /// </summary>
    public partial class Bill : UserControl
    {
        private AppDbContext Db = new AppDbContext();
        public Bill()
        {
            InitializeComponent();
            LoadBills();    
        }
        private void LoadBills()
        {
            BillTable.ItemsSource = Db.Bill
                .Select(b => new
                {
                    Id = b.Id,
                    Date = b.CreatedDate,
                    Customer = Db.Customer.FirstOrDefault(c => c.Id == b.CustomerId).customerName,
                    Total = b.TotalAmount,
                    Status = b.State
                }).ToList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            BillForm billForm = new BillForm();
            billForm.ShowDialog(); // Chặn tương tác với MainWindow
            LoadBills();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (BillTable.SelectedItem != null)
            {
                dynamic selectedBill = BillTable.SelectedItem;
                BillForm billForm = new BillForm(selectedBill.Id); // Chỉnh sửa hóa đơn
                billForm.ShowDialog();
                LoadBills();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần chỉnh sửa.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (BillTable.SelectedItem != null)
            {
                dynamic selectedBill = BillTable.SelectedItem;
                string billId = selectedBill.Id;

                var bill = Db.Bill.Find(billId);
                if (bill != null)
                {
                    // Xóa tất cả BillDetail trước khi xóa Bill
                    var billDetails = Db.BillDetail.Where(bd => bd.BillId == billId).ToList();
                    if (billDetails.Any())
                    {
                        Db.BillDetail.RemoveRange(billDetails);
                    }

                    Db.Bill.Remove(bill);
                    Db.SaveChanges();
                    LoadBills();
                    MessageBox.Show("Hóa đơn đã được xóa.");
                }
                else
                {
                    MessageBox.Show("Không tìm thấy hóa đơn để xóa.");
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xóa.");
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = "PDF Files|*.pdf" };
            if (saveFileDialog.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    Document doc = new Document();
                    PdfWriter.GetInstance(doc, fs);
                    doc.Open();

                    PdfPTable table = new PdfPTable(4);
                    table.AddCell("Mã hóa đơn");
                    table.AddCell("Ngày lập");
                    table.AddCell("Khách hàng");
                    table.AddCell("Tổng tiền");

                    foreach (dynamic item in BillTable.Items)
                    {
                        table.AddCell(item.Id.ToString());
                        table.AddCell(item.Date.ToString());
                        table.AddCell(item.Customer);
                        table.AddCell(item.Total.ToString());
                    }

                    doc.Add(table);
                    doc.Close();
                }
                MessageBox.Show("Xuất file PDF thành công.");
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim(); // Giả sử có TextBox tên SearchTextBox

            if (!string.IsNullOrEmpty(searchText))
            {
                var filteredBills = Db.Bill
                    .Where(b => b.Id.Contains(searchText) ||
                                Db.Customer.Any(c => c.Id == b.CustomerId && c.customerName.Contains(searchText)))
                    .Select(b => new
                    {
                        Id = b.Id,
                        Date = b.CreatedDate,
                        Customer = Db.Customer.FirstOrDefault(c => c.Id == b.CustomerId).customerName,
                        Total = b.TotalAmount,
                        Status = b.State
                    }).ToList();

                BillTable.ItemsSource = filteredBills;
            }
            else
            {
                LoadBills();
            }
        }
    }
}
