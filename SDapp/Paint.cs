using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;

namespace SoftwareDesign_2017
{
    class Paint
    {
        /// <summary>
        /// 画一条线段
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns>Path类型的一条线段</returns>
        public Path DrawLine(Point startPoint,Point endPoint)
        {
            LineGeometry lineGeometry = new LineGeometry();
            lineGeometry.StartPoint = startPoint;
            lineGeometry.EndPoint = endPoint;


            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
            myPath.Data = lineGeometry;

            return myPath;
        }

        public PathFigure GenerateLine(Point startPoint, Point endPoint)
        {
            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = startPoint;

            LineSegment myLineSegment = new LineSegment();
            myLineSegment.Point = endPoint;

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(myLineSegment);

            myPathFigure.Segments = myPathSegmentCollection;

            return myPathFigure;
        }


        public Path DrawCoordinate(Point originPoint,Point endPoint_x,Point endPoint_y)
        {                        
            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(GenerateLine(originPoint, endPoint_y)); 
            myPathFigureCollection.Add(GenerateLine(originPoint, endPoint_x));

            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = myPathFigureCollection;


            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
            myPath.Data = myPathGeometry;

            return myPath;
        }

        /// <summary>
        /// 输入的点集合的X应该为0 - 1000这种连续取值
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isFilled"></param>
        /// <param name="isClosed"></param>
        /// <returns></returns>
        public Path Draw(List<Point> values, bool isFilled, bool isClosed)
        {
            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(values[0], isFilled /* is filled */, isClosed /* is closed */);

                //画出一系列线段并且连接成曲线
                for (int i = 1; i < values.Count; i++)
                {
                    ctx.LineTo(values[i], true, false);
                }
                //使用bezier曲线（Bessel)进行连接
                /*Bezier bezier = new Bezier(values, 0.7);
                for (int i = 1; i < values.Count; i++)
                {
                   ctx.BezierTo(bezier.controlPointCollection[2 * i - 2], bezier.controlPointCollection[2 * i - 1], values[i], true, false);
                }*/
            }

            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
            myPath.Data = geometry;

            return myPath;
        }


        public Path DrawSin()
        {
            Point point = new Point();
            List<Point> points = new List<Point>();
            for (double i = 0; i < 1000; i++)
            {
                //创建y = sin(x)  
                point.X = 2 * i;//x
                point.Y = 40 * Math.Sin(i / 10);//y

                //坐标转换  
                XYTransfer(point,ref point);
                points.Add(point);
            }

            Path myPath = Draw(points, true, false); 

            return myPath;
        }

        private void XYTransfer(Point xyPoint,ref Point point)
        {
            point.X = xyPoint.X + 10;
            point.Y = - xyPoint.Y + 100;
        }
        
    }
}
