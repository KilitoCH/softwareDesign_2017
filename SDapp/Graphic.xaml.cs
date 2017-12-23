using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// Graphic.xaml 的交互逻辑
    /// </summary>
    public partial class Graphic : Window
    {
        private GraphicContext graphicContext = new GraphicContext();
        private double yMax = 0;
        private double yMin = 0;
        private double xMin = 0;
        private double gapX = 0;
        private double gapY = 0;

        public Graphic(List<Point> points,string yLabel)
        {
            Paint paint = new Paint();
            Visual visual;
            DataContext = graphicContext;
            InitializeComponent();
            if(yLabel.Equals("bpskPsd") || yLabel.Equals("bocPsd"))
                visual = paint.DrawVisual(CoordinateTransformDb(points), true, false, Type.Line);
            else if(yLabel.Equals("bpskAutocorrelation") || yLabel.Equals("bocAutocorrelation"))
                visual = paint.DrawVisual(CoordinateTransformRs(points), true, false, Type.Line);
            else
                visual = paint.DrawVisual(CoordinateTransformDb(points), true, false, Type.Line);
            drawingCanvas.RemoveAll();
            drawingCanvas.AddVisual(visual);
            graphicContext.LabelX_0 = xMin;
            graphicContext.LabelX_1 = graphicContext.LabelX_0 + gapX;
            graphicContext.LabelX_2 = graphicContext.LabelX_1 + gapX;
            graphicContext.LabelX_3 = 0;
            graphicContext.LabelX_4 = graphicContext.LabelX_3 + gapX;
            graphicContext.LabelX_5 = graphicContext.LabelX_4 + gapX;
            graphicContext.LabelX_6 = graphicContext.LabelX_5 + gapX;

            graphicContext.LabelY_0 = yMin;
            graphicContext.LabelY_1 = graphicContext.LabelY_0 + gapY;
            graphicContext.LabelY_2 = graphicContext.LabelY_1 + gapY;
            graphicContext.LabelY_3 = graphicContext.LabelY_2 + gapY;
            graphicContext.LabelY_4 = graphicContext.LabelY_3 + gapY;
            graphicContext.LabelY_5 = graphicContext.LabelY_4 + gapY;
            graphicContext.LabelY_6 = graphicContext.LabelY_5 + gapY;
            graphicContext.LabelY_7 = graphicContext.LabelY_6 + gapY;
            graphicContext.LabelY_8 = graphicContext.LabelY_7 + gapY;

            graphicContext.YLabel = yLabel;
        }

        /// <summary>
        /// 将真实的PsdDb点阵根据显示空间的大小调整为动态的适合窗口的点阵
        /// </summary>
        /// <param name="pointsReal"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private List<Point> CoordinateTransformDb(List<Point> pointsRealDb)
        {
            List<Point> pointsForView = new List<Point>();
            List<Point> pointsTemp = new List<Point>();
            double boundWidth = 30000000;
            Point origin = new Point(drawingCanvas.Width / 2, drawingCanvas.Height);
            foreach (var point in pointsRealDb)
            {
                pointsTemp.Add(new Point(point.X, point.Y + 100));
                if ((point.Y + 100) > yMax)
                    yMax = point.Y + 100;
            }
            yMin = -100;
            xMin = pointsRealDb[0].X / 1000000;
            gapX = (pointsRealDb[pointsRealDb.Count - 1].X - pointsRealDb[0].X) / 6000000;
            gapY = yMax / 8;
            double yScale = origin.Y / yMax;
            double xScale = origin.X / (boundWidth / 2);
            foreach (var point in pointsTemp)
            {
                pointsForView.Add(new Point(point.X * xScale + origin.X, -point.Y * yScale + origin.Y));
            }
            return pointsForView;            
        }

        
        private List<Point> CoordinateTransformRs(List<Point> pointsRs)
        {
            List<Point> pointsForView = new List<Point>();
            double timeDomain = 0.000002;
            Point origin = new Point(drawingCanvas.Width / 2, drawingCanvas.Height);
            yMax = 1;
            yMin = 0;
            xMin = pointsRs[0].X * 1000000;
            gapX = (pointsRs[pointsRs.Count - 1].X - pointsRs[0].X) * 1000000 / 6;
            gapY = 0.125;
            double yScale = origin.Y / yMax;
            double xScale = origin.X / (timeDomain / 2);
            foreach (var point in pointsRs)
            {
                pointsForView.Add(new Point(point.X * xScale + origin.X, -point.Y * yScale + origin.Y));
            }
            
            return pointsForView;
        }
    }
}
