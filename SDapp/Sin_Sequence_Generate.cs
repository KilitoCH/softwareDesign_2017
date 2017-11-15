using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SoftwareDesign_2017
{
    class Sin_Sequence_Generate
    {
        /// <summary>
        /// 在图形绘制、图片保存测试的时候提供一个Sin图像的离散序列
        /// </summary>
        /// <param name="frequence">正弦波的频率</param>
        /// <returns></returns>
        public List<Point> Sin_Sequence(Double frequence)
        {
            var points = new List<Point>();
            Point point = new Point();
            for (double i = 0; i < 1000; i++)
            {
                //创建y = sin(2 * pi * f * x)
                point.X = 0.5 * Math.PI * frequence * i;//x
                point.Y = 40 * Math.Sin(i / 10);//y

                //坐标转换  
                XYTransfer(point, ref point);
                points.Add(point);
            }
            return points;
        }

        /// <summary>
        /// 这个方法用来进行坐标变换，具体用途为将整个图像的起点变换到指定位置，该方法待完善
        /// </summary>
        /// <param name="xyPoint">起点需要变换到的位置</param>
        /// <param name="point">需要变换的点</param>
        private void XYTransfer(Point xyPoint, ref Point point)
        {
            point.X = xyPoint.X + 10;
            point.Y = -xyPoint.Y + 100;
        }
    }
}
