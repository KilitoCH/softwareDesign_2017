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
    public enum Type
    {
        Line,
        Bezier
    }
    class Paint
    {
        #region 字段
        public List<Point> points = new List<Point>();
#endregion
        #region 初始化函数
        public Paint()
        {
            GenerateSinSequence();
        }
#endregion
        /// <summary>
        /// 绘图函数绘出图像如果过于折现化说明样本点密度过低，请改用bezier函数连接以改善图形
        /// </summary>
        /// <param name="values">样本序列</param>
        /// <param name="isFilled"></param>
        /// <param name="isClosed">是否闭合</param>
        /// <returns></returns>
        private Geometry DrawGeometry(List<Point> values, bool isFilled, bool isClosed,Type type)
        {
            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext ctx = geometry.Open())
            {
                ctx.BeginFigure(values[0], isFilled /* is filled */, isClosed /* is closed */);

                //画出一系列线段并且连接成曲线
                if (type == Type.Line)
                {
                    for (int i = 1; i < values.Count; i++)
                    {
                        ctx.LineTo(values[i], true, false);
                    }
                }
                //使用bezier曲线（Bessel)进行连接
                else if (type == Type.Bezier)
                {
                    Bezier bezier = new Bezier(values, 0.7);
                    for (int i = 1; i < values.Count; i++)
                    {
                        ctx.BezierTo(bezier.controlPointCollection[2 * i - 2], bezier.controlPointCollection[2 * i - 1], values[i], true, false);
                    }
                }
            }

            return geometry;
        }

        /// <summary>
        /// 绘图函数绘出图像如果过于折现化说明样本点密度过低，请改用bezier函数连接以改善图形
        /// </summary>
        /// <param name="values"></param>
        /// <param name="isFilled"></param>
        /// <param name="isClosed"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Visual Draw(List<Point> values, bool isFilled, bool isClosed, Type type)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            var geometry = DrawGeometry(values, isFilled, isClosed, type);

            using (var pic = drawingVisual.RenderOpen())
            {
                pic.DrawGeometry(Brushes.White, new Pen(Brushes.Black, 1), geometry);
            }

            return drawingVisual;
        }

        /// <summary>
        /// 这个方法用来进行坐标变换，具体用途为将整个图像的起点变换到指定位置，该方法待完善
        /// </summary>
        /// <param name="xyPoint">起点需要变换到的位置</param>
        /// <param name="point">需要变换的点</param>
        private void XYTransfer(Point xyPoint,ref Point point)
        {
            point.X = xyPoint.X + 10;
            point.Y = - xyPoint.Y + 100;
        }
        #region 测试用代码
        /// <summary>
        /// 在图形绘制、图片保存测试的时候提供一个Sin图像的离散序列,这个序列可以用公开的TestSinSequence属性获取
        /// </summary>
        /// <returns></returns>
        private void GenerateSinSequence()
        {
            Point point = new Point();
            for (double i = 0; i < 1000; i++)
            {
                //创建y = sin(x)  
                point.X = 2 * i;//x
                point.Y = 40 * Math.Sin(i / 10);//y

                //坐标转换  
                XYTransfer(point, ref point);
                points.Add(point);
            }
        }

        /// <summary>
        /// 获取一个Sin的离散序列
        /// </summary>
        public List<Point> TestSinSequence
        {
            get
            {
                return points;
            }
        }
        
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
