using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SoftwareDesign_2017
{
    class Bezier
    {
        public List<Point> controlPointCollection = new List<Point>();

        /// <summary>
        /// 利用一个普通序列生成控制点序列
        /// </summary>
        /// <param name="list">原序列</param>
        /// <param name="scale">控制曲线平滑程度</param>
        public Bezier(List<Point> list,double scale)
        {
            ControlPointCollection(list,scale);
        }

        /// <summary>
        /// 利用一个普通序列生成控制点序列
        /// </summary>
        /// <param name="list">原序列</param>
        /// <param name="scale">控制曲线平滑程度</param>
        private void ControlPointCollection(List<Point> list,double scale)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Point[] controlPoint = new Point[2];                
                if (i == 0 || i == list.Count - 1)//第一个点和最后一个点直接作为控制点，因为没有其他的点进行辅助计算
                {
                    controlPointCollection.Add(list[i]);
                    continue;
                }
                else//其余的点采取取中点之后计算偏移量，再按照偏移量偏移的方法计算控制点，且每次可生成两个控制点
                {
                    controlPoint[0] = Average(list[i - 1], list[i]);
                    controlPoint[1] = Average(list[i], list[i + 1]);
                    Point ave = Average(controlPoint[0], controlPoint[1]);
                    Point offset = Offset(list[i], ave);
                    controlPoint[0] = Mul(Move(controlPoint[0], offset), list[i], scale);
                    controlPoint[1] = Mul(Move(controlPoint[1], offset), list[i], scale);
                    controlPointCollection.Add(controlPoint[0]);
                    controlPointCollection.Add(controlPoint[1]);
                }
            }
        }

        /// <summary>
        /// 计算出两个点的中点
        /// </summary>
        /// <param name="x">第一个点</param>
        /// <param name="y">第二个点</param>
        /// <returns>中点坐标</returns>
        private Point Average(Point x, Point y)
        {
            return new Point((x.X + y.X) / 2, (x.Y + y.Y) / 2);
        }

        /// <summary>
        ///使用指定的偏移量移动一个点
        /// </summary>
        /// <param name="offset">指定的偏移量</param>
        /// <param name="point">要移动的点</param>
        /// <returns>移动之后的点的坐标</returns>
        private Point Move(Point offset, Point point)
        {
            return new Point(offset.X + point.X, offset.Y + point.Y);
        }

        /// <summary>
        /// 计算将y移动到x需要的偏移量
        /// </summary>
        /// <param name="x">要移动到的位置</param>
        /// <param name="y">原来的位置</param>
        /// <returns>偏移量</returns>
        private Point Offset(Point x, Point y)
        {
            return new Point(x.X - y.X, x.Y - y.Y);
        }

        /// <summary>
        /// 使用scale参数控制控制点的位置
        /// </summary>
        /// <param name="x">当scale为1时的控制点</param>
        /// <param name="y">数据点</param>
        /// <param name="d">scale</param>
        /// <returns>修正后的控制点位置</returns>
        private Point Mul(Point x, Point y, double d)
        {
            Point temp = Offset(x, y);
            temp = new Point(temp.X * d, temp.Y * d);
            temp = Move(y, temp);
            return temp;
        }
    }
}
