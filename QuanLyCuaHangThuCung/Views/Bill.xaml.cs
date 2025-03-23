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
                    Note = b.Note
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

                    // Load font hỗ trợ tiếng Việt
                    string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                    BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font vietnameseFont = new Font(baseFont, 12, Font.NORMAL);

                    PdfPTable table = new PdfPTable(4);

                    // Thêm tiêu đề bảng với font hỗ trợ Unicode
                    table.AddCell(new PdfPCell(new Phrase("Mã hóa đơn", vietnameseFont)));
                    table.AddCell(new PdfPCell(new Phrase("Ngày lập", vietnameseFont)));
                    table.AddCell(new PdfPCell(new Phrase("Khách hàng", vietnameseFont)));
                    table.AddCell(new PdfPCell(new Phrase("Tổng tiền", vietnameseFont)));

                    foreach (dynamic item in BillTable.Items)
                    {
                        table.AddCell(new PdfPCell(new Phrase(item.Id.ToString(), vietnameseFont)));
                        table.AddCell(new PdfPCell(new Phrase(item.Date.ToString(), vietnameseFont)));
                        table.AddCell(new PdfPCell(new Phrase(item.Customer, vietnameseFont)));
                        table.AddCell(new PdfPCell(new Phrase(item.Total.ToString(), vietnameseFont)));
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
                                Db.Customer.Any(c => c.Id == b.CustomerId && c.customerName.Contains(searchText))
                                || b.CreatedDate.ToString().Contains(searchText))
                    .Select(b => new
                    {
                        Id = b.Id,
                        Date = b.CreatedDate,
                        Customer = Db.Customer.FirstOrDefault(c => c.Id == b.CustomerId).customerName,
                        Total = b.TotalAmount,
                        Note = b.Note
                    }).ToList();

                BillTable.ItemsSource = filteredBills;
            }
            else
            {
                LoadBills();
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            LoadBills();
        }

        private void BillTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Detail_Click(object sender, RoutedEventArgs e)
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
                MessageBox.Show("Vui lòng chọn hóa đơn cần xem.");
            }
        }
    }
}
