using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace QuanLyCuaHangThuCung.Views
{
    /// <summary>
    /// Interaction logic for BuyHistory.xaml
    /// </summary>
    public partial class BuyHistory : Window
    {
        private AppDbContext Db = new AppDbContext();
        public ObservableCollection<Models.BillDetail> BillDetails { get; set; } = new ObservableCollection<Models.BillDetail>();
        public ObservableCollection<Models.Customer> Customer { get; set; } = new ObservableCollection<Models.Customer>();

        public BuyHistory()
        {
            InitializeComponent();
        }

        public BuyHistory(Customer Id)
        {
            InitializeComponent();
        }

        private void LoadData(string cusID)
        {
            
        }

        private void SpDvTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
