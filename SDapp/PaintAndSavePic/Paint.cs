using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// 用于指定使用直线或者贝塞尔曲线对样本点进行连接
    /// </summary>
    public enum LineType
    {
        Line,//使用直线
        Bezier//使用bessel曲线
    }
    class Paint
    {
        /// <summary>
        /// 使用原始序列和指定的连接方式绘制几何图形
        /// </summary>
        /// <param name="points">样本序列</param>
        /// <param name="isFilled">是否填充</param>
        /// <param name="isClosed">是否闭合</param>
        /// <returns>连接得到的几何图形</returns>
        private Geometry DrawGeometry(List<Point> points, bool isFilled, bool isClosed,LineType type)
        {
            if (points != null)
            {
                StreamGeometry geometry = new StreamGeometry();

                using (StreamGeometryContext ctx = geometry.Open())//打开一个画板
                {
                    ctx.BeginFigure(points[0], isFilled /* is filled */, isClosed /* is closed */);//设定画图的起始点

                    //画出一系列线段并且以折线的形式顺次连接
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
                return geometry;//返回得到的点列
            }
            else
                throw new Exception("请先输入参数");
        }

        /// <summary>
        /// 绘图函数绘出图像如果过于折线化说明样本点密度过低，请改用bezier函数连接以改善图形
        /// </summary>
        /// <param name="points">原始的点序列</param>
        /// <param name="isFilled">是否填充</param>
        /// <param name="isClosed">是否闭合，即将第一个点和最后一个点进行连接</param>
        /// <param name="type">指定连接方式</param>
        /// <returns>连接得到的visual对象</returns>
        public Visual DrawVisual(List<Point> points, bool isFilled, bool isClosed, LineType type,Pen pen)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            var geometry = DrawGeometry(points, isFilled, isClosed, type);

            using (var pic = drawingVisual.RenderOpen())
            {
                pic.DrawGeometry(null, pen, geometry);//使用指定的画笔将geometry对象绘制为visual对象
            }

            return drawingVisual;
        }
    }
}
