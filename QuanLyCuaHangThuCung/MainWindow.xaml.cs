using QuanLyCuaHangThuCung.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuanLyCuaHangThuCung
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new Home();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Home();
        }

        private void Product_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SanPham();
        }

        private void Service_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Service();
        }

        private void Customer_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Customer();
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Report();
        }

        private void Employee_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Employee();
        }

        private void Bill_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Bill();
        }
        
        private void DX_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
