using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace QuanLyCuaHangThuCung.Views
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl
    {
        public PlotModel MyPlotModel { get; set; }
        public Report()
        {
            InitializeComponent();
            MyPlotModel = new PlotModel { Title = "Biểu đồ cột" };

            var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom };
            categoryAxis.Labels.AddRange(new List<string> { "A", "B", "C", "D" });

            var valueAxis = new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 100 };

            var barSeries = new BarSeries
            {
                ItemsSource = new List<BarItem> {
                    new BarItem { Value = 30 },
                    new BarItem { Value = 70 },
                    new BarItem { Value = 50 },
                    new BarItem { Value = 90 }
                }
            };


            MyPlotModel.Axes.Add(categoryAxis);
            MyPlotModel.Axes.Add(valueAxis);
            MyPlotModel.Series.Add(barSeries);
        }

        private void DoanhThu_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
