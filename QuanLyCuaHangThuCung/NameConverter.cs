using QuanLyCuaHangThuCung.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QuanLyCuaHangThuCung
{
    public class NameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var detail = value as BillDetail;
            if (detail == null)
            {
                Console.WriteLine("Convert: BillDetail bị null!");
                return "N/A";
            }

            Console.WriteLine($"Convert: Service = {detail.Service?.serviceName}, Product = {detail.Product?.productName}");

            return detail.Service?.serviceName ?? detail.Product?.productName ?? "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
