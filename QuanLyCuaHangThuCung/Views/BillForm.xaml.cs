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
using System.Diagnostics;

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
        public ObservableCollection<Models.Employee> Employees { get; set; } = new ObservableCollection<Models.Employee>();
        public ObservableCollection<Models.Service> Services { get; set; } = new ObservableCollection<Models.Service>();
        public ObservableCollection<Models.BillDetail> BillDetails { get; set; } = new ObservableCollection<Models.BillDetail>();
        public BillForm(string billId, bool isReadOnly = false)
        {
            InitializeComponent();
            LoadCustomers();
            LoadProducts();
            LoadServices();
            LoadEmployees();

            if (isReadOnly)
            {
                SaveBTN.IsEnabled = false;
                addSP.IsEnabled = false;
                addDv.IsEnabled = false;
                delDV.IsEnabled = false;
                XoaSPBtn.IsEnabled = false;
                cbKhachHang.IsEnabled = false;
                cbNhanVien.IsEnabled = false;
                serviceList.IsEnabled = false;
            }

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
        private void LoadEmployees()
        {
            Employees.Clear();
            var employeeList = Db.Employee.ToList();

            // Kiểm tra nếu không có dữ liệu
            if (employeeList.Count == 0)
            {
                MessageBox.Show("Không có nhân viên nào trong database!");
                return;
            }

            foreach (var employee in employeeList)
            {
                Employees.Add(employee);
            }

            cbNhanVien.ItemsSource = Employees;
            cbNhanVien.DisplayMemberPath = "employeeName";
            cbNhanVien.SelectedValuePath = "Id";

            MessageBox.Show($"Đã tải {Employees.Count} nhân viên!");
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
            TileBillForm.Text = "Chi tiết hóa đơn";
            currentBill = Db.Bill
                .Include(b => b.Employee)
                .Include(b => b.Customer)
                .FirstOrDefault(b => b.Id == billId); // Dùng Include để load quan hệ

            if (currentBill != null)
            {
                BillDetails = new ObservableCollection<BillDetail>(
                    Db.BillDetail
                    .Where(bd => bd.BillId == billId)
                    .Include(bd => bd.Product) // Load Product
                    .Include(bd => bd.Service) // Load Service
                );
                SpDvTable.ItemsSource = BillDetails;
                totalAmount = BillDetails.Sum(bd => bd.TotalPrice);
                UpdateTotal(); // Cập nhật hiển thị tổng tiền

                // Kiểm tra thông tin nhân viên
                if (currentBill.Employee != null)
                {
                    cbNhanVien.SelectedItem = cbNhanVien.Items
                        .Cast<Models.Employee>()
                        .FirstOrDefault(e => e.Id == currentBill.EmployeeId);
                    Debug.WriteLine($"Nhân viên: {currentBill.Employee.employeeName}");
                }
                else
                {
                    Debug.WriteLine("Không tìm thấy nhân viên.");
                }

                // Kiểm tra thông tin khách hàng
                if (currentBill.Customer != null)
                {
                    rbKhachQuen.IsChecked = true;
                    rbKhachQuen.IsEnabled = false;
                    rbKhachVangLai.IsEnabled = false;
                    cbKhachHang.SelectedItem = cbKhachHang.Items
                        .Cast<Models.Customer>()
                        .FirstOrDefault(c => c.Id == currentBill.CustomerId);
                    Debug.WriteLine($"Khách hàng: {currentBill.Customer.customerName}");
                }
                else
                {
                    rbKhachVangLai.IsChecked = true;
                    rbKhachQuen.IsEnabled = false;
                    rbKhachVangLai.IsEnabled = false;
                    Debug.WriteLine("Không tìm thấy khách hàng.");
                }
            }
        }

        public BillForm()
        {
            InitializeComponent();
            LoadCustomers();
            LoadEmployees();
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
                if (quantity <= 0)
                {
                    MessageBox.Show("Số lượng phải lớn hơn 0!");
                    return;
                }

                // Kiểm tra số lượng tồn kho
                if (SelectedProduct.quantity < quantity)
                {
                    MessageBox.Show("Số lượng hiện có không đủ!");
                    return;
                }

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

                SelectedProduct.quantity -= quantity;
                Db.Entry(SelectedProduct).State = EntityState.Modified;
                Db.SaveChanges();

                BillDetails.Add(detail);
                SpDvTable.Items.Refresh(); // Cập nhật bảng
                LoadProducts(); // Cập nhật danh sách sản phẩm
                UpdateTotal();
            }
            resetInput();
        }


        private void DelSP_Click(object sender, RoutedEventArgs e)
        {
            if (SpDvTable.SelectedItem is BillDetail selected)
            {
                totalAmount -= selected.TotalPrice;
                if (selected.Product != null)
                {
                    var product = Db.Product.Find(selected.Product.Id);
                    if (product != null)
                    {
                        product.quantity += selected.Quantity;
                        Db.Entry(product).State = EntityState.Modified;
                        Db.SaveChanges();
                    }
                }
                BillDetails.Remove(selected);
                SpDvTable.Items.Refresh();
                LoadProducts();
                UpdateTotal();
                SpDvTable.SelectedIndex = -1; // Hủy chọn
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

            totalLabel.Content = $"Tổng tiền: {totalAmount - discount:N0} VNĐ";
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
            currentBill.EmployeeId = cbNhanVien.SelectedValue?.ToString();

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

                    // Load font hỗ trợ tiếng Việt
                    string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "times.ttf");
                    BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font vietnameseFont = new Font(baseFont, 12, Font.NORMAL);

                    doc.Add(new Paragraph($"Hóa đơn: {currentBill.Id}", vietnameseFont));
                    doc.Add(new Paragraph($"Ngày: {currentBill.CreatedDate}", vietnameseFont));
                    doc.Add(new Paragraph($"Tổng tiền: {totalAmount:C}", vietnameseFont));

                    PdfPTable table = new PdfPTable(3);

                    // Thêm tiêu đề bảng với font hỗ trợ tiếng Việt
                    table.AddCell(new PdfPCell(new Phrase("Tên sản phẩm/Dịch vụ", vietnameseFont)));
                    table.AddCell(new PdfPCell(new Phrase("Số lượng", vietnameseFont)));
                    table.AddCell(new PdfPCell(new Phrase("Giá", vietnameseFont)));

                    foreach (BillDetail bd in SpDvTable.Items)
                    {
                        table.AddCell(new PdfPCell(new Phrase(bd.ProductId ?? bd.ServiceId, vietnameseFont)));
                        table.AddCell(new PdfPCell(new Phrase(bd.Quantity.ToString(), vietnameseFont)));
                        table.AddCell(new PdfPCell(new Phrase(bd.TotalPrice.ToString(), vietnameseFont)));
                    }

                    doc.Add(table);
                    doc.Close();
                }
                MessageBox.Show("Xuất hóa đơn thành công.");
            }
        }
        private void SpDvTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpDvTable.SelectedItem is BillDetail selectedDetail)
            {
                if (selectedDetail.Product != null) // Nếu là sản phẩm
                {
                    tenSP_input.Text = selectedDetail.Product.productName;
                    price_input.Text = selectedDetail.UnitPrice.ToString();
                    SL_input.Text = selectedDetail.Quantity.ToString();
                    serviceList.SelectedIndex = -1; // Hủy chọn dịch vụ
                }
                else if (selectedDetail.Service != null) // Nếu là dịch vụ
                {
                    tenSP_input.Text = "";
                    price_input.Text = "";
                    SL_input.Text = "0"; // Dịch vụ không có số lượng
                    serviceList.SelectedItem = selectedDetail.Service;
                }
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
