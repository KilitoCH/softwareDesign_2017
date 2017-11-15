using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// 这个类存储了在主界面中画图的时候产生的点阵序列和Visual对象
    /// </summary>
    class Context
    {

        public List<Point> pointsForTabItem1 = new List<Point>();//选项卡1的图片的点阵
        public List<Point> pointsForTabItem2 = new List<Point>();//选项卡2的图片的点阵
        public List<Point> pointsForTabItem3 = new List<Point>();//选项卡3的图片的点阵

        public Visual visualForTabItem1;//选项卡1的图片的Visual格式字段
        public Visual visualForTabItem2;//选项卡2的图片的Visual格式字段
        public Visual visualForTabItem3;//选项卡3的图片的Visual格式字段
    }
}
