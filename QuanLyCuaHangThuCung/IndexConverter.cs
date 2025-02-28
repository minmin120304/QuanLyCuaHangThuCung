using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Controls;

public class IndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Lấy số thứ tự từ DataGridRow
        if (value is DataGridRow row)
        {
            return row.GetIndex() + 1; // Cộng 1 để STT bắt đầu từ 1
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
