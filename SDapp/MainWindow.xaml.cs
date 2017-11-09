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
            Paint paint = new Paint();
            //tabItem2_canvas.AddVisual(paint.Draw(paint.TestSinSequence,true,false,Type.Line));
            SavePic savePic = new SavePic();
            savePic.SaveVisual(paint.Draw(paint.TestSinSequence, true, false, Type.Line));
            //tabItem2_grid.Children.Add(paint.DrawCoordinate(new Point(10, 400), new Point(500, 400), new Point(10, 10)));//笛卡尔坐标系
        }
    }
}
