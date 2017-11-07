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
        /// 
        /// </summary>
        /// <param name="list">the point list</param>
        /// <param name="scale">use this param to control the angle of silde</param>
        public Bezier(List<Point> list,double scale)
        {
            ControlPointCollection(list,scale);
        }

        private void ControlPointCollection(List<Point> list,double scale)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Point[] controlPoint = new Point[2];                
                if (i == 0 || i == list.Count - 1)
                {
                    controlPointCollection.Add(list[i]);
                    continue;
                }
                else
                {
                    controlPoint[0] = Average(list[i - 1], list[i]);
                    controlPoint[1] = Average(list[i], list[i + 1]);
                    Point ave = Average(controlPoint[0], controlPoint[1]);
                    Point offset = Offset(list[i], ave);
                    controlPoint[0] = Mul(Move(controlPoint[0], offset), list[i], scale);
                    controlPoint[1] = Mul(Move(controlPoint[1], offset), list[i], scale);
                }                    
                
                controlPointCollection.Add(controlPoint[0]);
                controlPointCollection.Add(controlPoint[1]);
            }
        }

        private Point Average(Point x, Point y)
        {
            return new Point((x.X + y.X) / 2, (x.Y + y.Y) / 2);
        }

        /// <summary>
        /// move point with offset
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point Move(Point offset, Point point)
        {
            return new Point(offset.X + point.X, offset.Y + point.Y);
        }

        /// <summary>
        /// calculate offset to move y to x
        /// </summary>
        /// <param name="x">target point</param>
        /// <param name="y">operated point</param>
        /// <returns></returns>
        private Point Offset(Point x, Point y)
        {
            return new Point(x.X - y.X, x.Y - y.Y);
        }

        /// <summary>
        /// use d to control the controlPoint
        /// </summary>
        /// <param name="x">the control point when d=1</param>
        /// <param name="y">data point</param>
        /// <param name="d">scale</param>
        /// <returns></returns>
        private Point Mul(Point x, Point y, double d)
        {
            Point temp = Offset(x, y);
            temp = new Point(temp.X * d, temp.Y * d);
            temp = Move(y, temp);
            return temp;
        }
    }
}
