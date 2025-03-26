using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace QuanLyCuaHangThuCung.Views
{
    public class AlternationIndexToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                // Dòng chẵn: màu trắng, dòng lẻ: xanh nhạt
                return index % 2 == 0 ? Brushes.White : (Brush)new BrushConverter().ConvertFrom("#E3F2FD");
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
