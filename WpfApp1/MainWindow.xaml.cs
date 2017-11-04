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

namespace SoftwareDesign_2017
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FuncPaint funcPaint = new FuncPaint();
            //tabItem1_grid.Children.Add(funcPaint.DrawCoordinate(new Point(10, 400), new Point(500, 400), new Point(10, 10)));//笛卡尔坐标系
            tabItem1_grid.Children.Add(funcPaint.DrawSin());
        }
    }
}
