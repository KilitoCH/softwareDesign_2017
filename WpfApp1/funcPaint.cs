using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace SoftwareDesign_2017
{
    class FuncPaint
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

        private StreamGeometry BuildRegularPolygon(Point[] values, bool isClosed, bool isfilled)
        {
            // c is the center, r is the radius,  
            // numSides the number of sides, offsetDegree the offset in Degrees.  
            // Do not add the last point.  

            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(values[0], isfilled /* is filled */, isClosed /* is closed */);

                for (int i = 1; i < values.Length; i++)
                {
                    ctx.LineTo(values[i], true /* is stroked */, false /* is smooth join */);
                }
            }

            return geometry;

        }

        public Path DrawSin()
        {
            Point point = new Point();
            Point[] points = new Point[1000];
            for (int i = 1; i < 1000; i++)
            {
                //计算sin（x，y）  
                point.X = i;//x  
                point.Y = Math.Sin(i);//y  

                //坐标转换  
                XYTransfer(point,ref point);
                points[i] = point;

            }

            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
            StreamGeometry theGeometry = BuildRegularPolygon(points, true, false);
            myPath.Data = theGeometry;

            return myPath;
        }

        private void XYTransfer(Point xyPoint,ref Point point)
        {
            point.X = xyPoint.X;
            point.Y = xyPoint.Y + 100;
        }
        
    }
}
