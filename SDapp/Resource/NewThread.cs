using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// 用来实现向新的窗口线程传递参数的类
    /// </summary>
    class NewThread
    {
        public void StartThread()
        {
            Graphic graphic = new Graphic(points,whichFigure);
            graphic.ShowDialog();
        }

        public List<Point> points = new List<Point>();
        public string whichFigure;
    }
}
