using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// 用于指定使用直线或者贝塞尔曲线对样本点进行连接
    /// </summary>
    public enum LineType
    {
        Line,
        Bezier
    }
    class Paint
    {
        /// <summary>
        /// 绘图函数绘出图像如果过于折现化说明样本点密度过低，请改用bezier函数连接以改善图形
        /// </summary>
        /// <param name="points">样本序列</param>
        /// <param name="isFilled"></param>
        /// <param name="isClosed">是否闭合</param>
        /// <returns></returns>
        private Geometry DrawGeometry(List<Point> points, bool isFilled, bool isClosed,LineType type)
        {
            try
            {
                StreamGeometry geometry = new StreamGeometry();

                using (StreamGeometryContext ctx = geometry.Open())
                {
                    ctx.BeginFigure(points[0], isFilled /* is filled */, isClosed /* is closed */);

                    //画出一系列线段并且连接成曲线
                    if (type == LineType.Line)
                    {
                        for (int i = 1; i < points.Count && points[i].X <= 480; i++)
                        {
                            ctx.LineTo(points[i], true, false);
                        }
                    }
                    //使用bezier曲线进行连接
                    else if (type == LineType.Bezier)
                    {
                        Bezier bezier = new Bezier(points, 0.7);
                        for (int i = 1; i < points.Count; i++)
                        {
                            ctx.BezierTo(bezier.controlPointCollection[2 * i - 2], bezier.controlPointCollection[2 * i - 1], points[i], true, false);
                        }
                    }
                }

                return geometry;
            }
            catch (Exception)
            {
                MessageBox.Show("请先输入频率");
                return null;
            }
            
        }

        /// <summary>
        /// 绘图函数绘出图像如果过于折线化说明样本点密度过低，请改用bezier函数连接以改善图形
        /// </summary>
        /// <param name="points"></param>
        /// <param name="isFilled"></param>
        /// <param name="isClosed"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Visual DrawVisual(List<Point> points, bool isFilled, bool isClosed, LineType type,Pen pen)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            var geometry = DrawGeometry(points, isFilled, isClosed, type);

            using (var pic = drawingVisual.RenderOpen())
            {
                pic.DrawGeometry(null, pen, geometry);
            }

            return drawingVisual;
        }

#region 测试用代码        
        private Path DrawLine(Point startPoint, Point endPoint)
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

        private PathFigure GenerateLine(Point startPoint, Point endPoint)
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


        private Path DrawCoordinate(Point originPoint, Point endPoint_x, Point endPoint_y)
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
        #endregion
    }
}
