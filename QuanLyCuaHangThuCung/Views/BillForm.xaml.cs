using iTextSharp.text.pdf;
using iTextSharp.text;
using QuanLyCuaHangThuCung.Models;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;
using Paragraph = iTextSharp.text.Paragraph;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text;

namespace QuanLyCuaHangThuCung.Views
{
    /// <summary>
    /// Interaction logic for BillForm.xaml
    /// </summary>
    public partial class BillForm : Window
    {
        private AppDbContext Db = new AppDbContext();
        private Models.Bill currentBill;
        private decimal totalAmount = 0;
        private Product SelectedProduct;
        public ObservableCollection<Models.Customer> Customers { get; set; } = new ObservableCollection<Models.Customer>();
        public ObservableCollection<Models.Service> Services { get; set; } = new ObservableCollection<Models.Service>();
        public ObservableCollection<Models.BillDetail> BillDetails { get; set; } = new ObservableCollection<Models.BillDetail>();
        public BillForm(string billId)
        {
            InitializeComponent();
            LoadCustomers();
            LoadProducts();
            LoadServices();

            SpDvTable.ItemsSource = BillDetails; // Gán dữ liệu vào bảng

            if (billId != null)
            {
                LoadBillData(billId);
            }
            else
            {
                currentBill = new Models.Bill
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedDate = DateTime.Now
                };
            }
            this.DataContext = currentBill;
        }
        private void LoadCustomers()
        {
            Customers.Clear();
            var customerList = Db.Customer.ToList();

            // Kiểm tra nếu không có dữ liệu
            if (customerList.Count == 0)
            {
                MessageBox.Show("Không có khách hàng nào trong database!");
                return;
            }

            foreach (var customer in customerList)
            {
                Customers.Add(customer);
            }

            cbKhachHang.ItemsSource = Customers;
            cbKhachHang.DisplayMemberPath = "customerName";
            cbKhachHang.SelectedValuePath = "Id";

            MessageBox.Show($"Đã tải {Customers.Count} khách hàng!");
        }

        private void LoadProducts()
        {
            ProductTable.ItemsSource = Db.Product.ToList();
        }

        private void LoadServices()
        {
            Services.Clear();
            var DSdichvu = Db.Service.ToList();

            // Kiểm tra nếu không có dữ liệu
            if (DSdichvu.Count == 0)
            {
                MessageBox.Show("Không có dịch vụ nào trong database!");
                return;
            }

            foreach (var service in DSdichvu)
            {
                Services.Add(service);
            }

            serviceList.ItemsSource = Services;
            serviceList.DisplayMemberPath = "serviceName";
            serviceList.SelectedValuePath = "Id";

            MessageBox.Show($"Đã tải {Services.Count} dịch vụ!");
        }

        private void LoadBillData(string billId)
        {
            currentBill = Db.Bill.Find(billId);
            if (currentBill != null)
            {
                BillDetails = new ObservableCollection<BillDetail>(
                    Db.BillDetail
                    .Where(bd => bd.BillId == billId)
                    .Include(bd => bd.Product) // Load Product
                    .Include(bd => bd.Service) // Load Service
                );
                SpDvTable.ItemsSource = BillDetails;
            }
        }

        public BillForm()
        {
            InitializeComponent();
            LoadCustomers();
            LoadProducts();
            LoadServices();
            this.DataContext = this;
        }

        private void resetInput()
        {
            serviceList.SelectedIndex = -1;
            tenSP_input.Text = "";
            price_input.Text = "";
            SL_input.Text = "";
        }

        private void AddDV_Click(object sender, RoutedEventArgs e)
        {
            if (serviceList.SelectedItem != null)
            {
                var service = (Models.Service)serviceList.SelectedItem;
                if (service == null)
                {
                    MessageBox.Show("Lỗi: Không tìm thấy dịch vụ.");
                    return;
                }
                totalAmount += service.price;

                var detail = new BillDetail
                {
                    ServiceId = service?.Id,
                    ProductId = null,
                    Service = service,  // Đảm bảo gán đúng
                    Product = null,
                    Quantity = 1,
                    UnitPrice = service?.price ?? 0,
                    TotalPrice = service?.price ?? 0
                };

                Console.WriteLine($"Thêm dịch vụ: {detail.Service?.serviceName}"); // Kiểm tra giá trị

                BillDetails.Add(detail);
                SpDvTable.Items.Refresh(); // Cập nhật bảng

                UpdateTotal();
            }
            resetInput();
        }

        private void DelDV_Click(object sender, RoutedEventArgs e)
        {
            if (SpDvTable.SelectedItem != null)
            {
                var selected = (BillDetail)SpDvTable.SelectedItem;
                totalAmount -= selected.TotalPrice;
                BillDetails.Remove(selected);
                SpDvTable.Items.Refresh();
                UpdateTotal();
            }
            resetInput();
        }
        private void ProductTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductTable.SelectedItem is Product product)
            {
                SelectedProduct = product;
                tenSP_input.Text = SelectedProduct.productName;
                price_input.Text = SelectedProduct.price.ToString();
                SL_input.Text = "0";
            }
        }
        private void AddSP_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct != null && int.TryParse(SL_input.Text, out int quantity))
            {
                totalAmount += SelectedProduct.price * quantity;

                var detail = new BillDetail
                {
                    ProductId = SelectedProduct.Id,
                    ServiceId = null,
                    Service = null,
                    Product = SelectedProduct, // Đảm bảo gán đúng
                    Quantity = quantity,
                    UnitPrice = SelectedProduct.price,
                    TotalPrice = SelectedProduct.price * quantity
                };

                // Debug
                Console.WriteLine($"Thêm Sản phẩm: {detail.Product?.productName}, Dịch vụ: {detail.Service?.serviceName}");

                BillDetails.Add(detail);
                SpDvTable.Items.Refresh(); // Cập nhật bảng

                UpdateTotal();
            }
            resetInput();
        }


        private void DelSP_Click(object sender, RoutedEventArgs e)
        {
            if (SpDvTable.SelectedItem != null)
            {
                var selected = (BillDetail)SpDvTable.SelectedItem;
                totalAmount -= selected.TotalPrice;
                BillDetails.Remove(selected);
                SpDvTable.Items.Refresh();
                UpdateTotal();
            }
            resetInput();
        }
        private void UpdateTotal()
        {
            decimal discount = 0;
            if (decimal.TryParse(chietKhau_input.Text, out decimal discountValue))
            {
                discount = Math.Round(totalAmount * (discountValue / 100), 2);
            }

            totalLabel.Content = $"Tổng tiền: {totalAmount - discount:C}";
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu currentBill đang null thì tạo mới
            if (currentBill == null)
            {
                currentBill = new Models.Bill();
            }

            currentBill.TotalAmount = totalAmount;
            currentBill.CustomerId = cbKhachHang.SelectedValue?.ToString();

            // Nếu Bill chưa có trong database, cần lưu trước
            if (Db.Bill.Find(currentBill.Id) == null)
            {
                Db.Bill.Add(currentBill);
                Db.SaveChanges(); // Lưu hóa đơn trước
            }

            // Gán ID hóa đơn vào từng chi tiết hóa đơn
            foreach (var detail in BillDetails)
            {
                detail.BillId = currentBill.Id; // Đảm bảo mỗi chi tiết có BillId hợp lệ
                var existingDetail = Db.BillDetail.Find(detail.Id);
                if (existingDetail == null)
                {
                    Db.BillDetail.Add(detail);
                }
            }
            Db.SaveChanges();
            //if (BillDetails.Any())
            //{
            //    foreach (var detail in BillDetails)
            //    {
            //        if (string.IsNullOrEmpty(detail.BillId))
            //        {
            //            MessageBox.Show("Lỗi: Mỗi chi tiết hóa đơn phải có một BillId hợp lệ.");
            //            return;
            //        }
            //    }
            //    Db.BillDetail.AddRange(BillDetails);
            //}

            //try
            //{
            //    Db.SaveChanges();
            //    MessageBox.Show("Hóa đơn đã được lưu.");
            //}
            //catch (DbEntityValidationException ex)
            //{
            //    StringBuilder sb = new StringBuilder();
            //    foreach (var validationErrors in ex.EntityValidationErrors)
            //    {
            //        foreach (var validationError in validationErrors.ValidationErrors)
            //        {
            //            string errorMsg = $"Property: {validationError.PropertyName} - Error: {validationError.ErrorMessage}";
            //            Console.WriteLine(errorMsg);
            //            sb.AppendLine(errorMsg);
            //        }
            //    }
            //    MessageBox.Show(sb.ToString(), "Lỗi Validation", MessageBoxButton.OK, MessageBoxImage.Error);
            //}

            MessageBox.Show("Hóa đơn đã được lưu.");
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
                    if (currentBill == null)
                    {
                        MessageBox.Show("Chưa có hóa đơn để xuất.");
                        return;
                    }
                    doc.Add(new Paragraph($"Hóa đơn: {currentBill.Id}"));
                    doc.Add(new Paragraph($"Ngày: {currentBill.CreatedDate}"));
                    doc.Add(new Paragraph($"Tổng tiền: {totalAmount:C}"));

                    PdfPTable table = new PdfPTable(3);
                    table.AddCell("Tên sản phẩm/Dịch vụ");
                    table.AddCell("Số lượng");
                    table.AddCell("Giá");

                    foreach (BillDetail bd in SpDvTable.Items)
                    {
                        table.AddCell(bd.ProductId ?? bd.ServiceId);
                        table.AddCell(bd.Quantity.ToString());
                        table.AddCell(bd.TotalPrice.ToString());
                    }

                    doc.Add(table);
                    doc.Close();
                }
                MessageBox.Show("Xuất hóa đơn thành công.");
            }
        }

        private void rbKhachQuen_CheckedChanged(object sender, EventArgs e)
        {
            if (rbKhachQuen.IsChecked == true)
            {
                cbKhachHang.IsEnabled = true;  // Hiển thị combobox
            }
            else
            {
                cbKhachHang.IsEnabled = false; // Ẩn combobox
            }
        }

        private void rbKhachVangLai_CheckedChanged(object sender, EventArgs e)
        {
            if (rbKhachVangLai.IsChecked == true)
            {
                cbKhachHang.IsEnabled = false; // Ẩn combobox
            }
        }

    }
}
