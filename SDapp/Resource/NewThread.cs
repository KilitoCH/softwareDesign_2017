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
            Graphic graphic = new Graphic(dictionary,WhichMode);
            graphic.ShowDialog();
        }

        public Dictionary<string, List<Point>> dictionary = new Dictionary<string, List<Point>>(); 
        
        public string WhichMode { set; get; }
    }
}
