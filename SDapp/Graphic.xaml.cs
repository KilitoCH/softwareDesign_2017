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
using Microsoft.Win32;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// Graphic.xaml 的交互逻辑
    /// </summary>
    public partial class Graphic : Window
    {
        private GraphicContext graphicContext = new GraphicContext();
        private Dictionary<string, List<Point>> dictionary;
        private Color color = new Color();
        private int flag;
        private double yMax = 0;
        private double yMin = 0;
        private double xMin = 0;
        private double gapX = 0;
        private double gapY = 0;

        public Graphic(Dictionary<string,List<Point>> dictionary,string yLabel)
        {
            List<Visual> visualList = new List<Visual>();
            this.dictionary = dictionary;
            List<List<Point>> pointsList = new List<List<Point>>();
            Paint paint = new Paint();
            DataContext = graphicContext;
            InitializeComponent();
            if (yLabel.Equals("功率谱密度"))
            {
                flag = 0;
                pointsList = CoordinateTransformDb(dictionary);
                for (int i = 0; i < pointsList.Count; i++)
                {                    
                    visualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i],1)));
                }
                graphicContext.YLabel = yLabel + "(dB)";
                graphicContext.XLabel = "频率(MHz)";
            }
            else if (yLabel.Equals("自相关函数"))
            {
                flag = 1;
                pointsList = CoordinateTransformRs(dictionary);
                for (int i = 0; i < pointsList.Count; i++)
                {
                    visualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1)));
                }
                graphicContext.YLabel = yLabel;
                graphicContext.XLabel = "时延(microseconds)";
            }
            else if (yLabel.Equals("s曲线"))
            {
                flag = 2;
                pointsList = CoordinateTransformError(dictionary,new Point(drawingCanvas.Width,drawingCanvas.Height / 2));
                for (int i = 0; i < pointsList.Count; i++)
                {
                    visualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1)));
                }
                graphicContext.YLabel = yLabel;
                graphicContext.XLabel = "时延(microseconds)";
            }
            else if (yLabel.Equals("码跟踪精度(时延)"))
            {
                flag = 3;
                pointsList = CoordinateTransformRs(dictionary);
                for (int i = 0; i < pointsList.Count; i++)
                {
                    visualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1)));
                }
                graphicContext.YLabel = yLabel;
                graphicContext.XLabel = "时延(microseconds)";
            }
            else if (yLabel.Equals("码跟踪精度(信噪比)"))
            {
                flag = 4;
                pointsList = CoordinateTransformRs(dictionary);
                for (int i = 0; i < pointsList.Count; i++)
                {
                    visualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1)));
                }
                graphicContext.YLabel = yLabel;
                graphicContext.XLabel = "信噪比(dB)";
            }
            drawingCanvas.RemoveAll();
            for (int i = 0; i < dictionary.Count; i++)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = dictionary.ElementAt(i).Key;
                listBoxItem.Foreground = color.colors[i];
                Setter setter = new Setter(FontSizeProperty, 7, "listBoxItem");
                listBox.Items.Add(listBoxItem);
                
            }
            foreach (var visual in visualList)
            {
                drawingCanvas.AddVisual(visual);
            }

            graphicContext.LabelX_0 = xMin;
            graphicContext.LabelX_1 = graphicContext.LabelX_0 + gapX;
            graphicContext.LabelX_2 = graphicContext.LabelX_1 + gapX;
            graphicContext.LabelX_3 = graphicContext.LabelX_2 + gapX;
            graphicContext.LabelX_4 = graphicContext.LabelX_3 + gapX;
            graphicContext.LabelX_5 = graphicContext.LabelX_4 + gapX;
            graphicContext.LabelX_6 = graphicContext.LabelX_5 + gapX;
        }

        /// <summary>
        /// 将真实的PsdDb点阵根据显示空间的大小调整为动态的适合窗口的点阵
        /// </summary>
        /// <param name="pointsReal"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private List<List<Point>> CoordinateTransformDb(Dictionary<string, List<Point>> dictionary)
        {
            List<List<Point>> pointsList = new List<List<Point>>();
            Point origin = new Point(drawingCanvas.Width / 2, drawingCanvas.Height);
            foreach (var item in dictionary)
            {
                foreach (var point in item.Value)
                {
                    if ((point.Y + 100) > yMax)
                        yMax = point.Y + 100;
                }               
            }
            yMin = -100;
            xMin = dictionary.First().Value[0].X / 1000000;
            gapX = (dictionary.First().Value[dictionary.First().Value.Count - 1].X - dictionary.First().Value[0].X) / 6000000;
            gapY = yMax / 8;
            double yScale = origin.Y / yMax;
            double xScale = drawingCanvas.Width / (dictionary.First().Value[dictionary.First().Value.Count - 1].X - dictionary.First().Value[0].X);
            foreach (var item in dictionary)
            {
                List<Point> pointsTemp = new List<Point>();
                foreach (var point in item.Value)
                {
                    pointsTemp.Add(new Point(point.X * xScale + origin.X, -(point.Y + 100) * yScale + origin.Y));
                }
                pointsList.Add(pointsTemp);
            }

            graphicContext.LabelY_0 = yMin;
            graphicContext.LabelY_1 = graphicContext.LabelY_0 + gapY;
            graphicContext.LabelY_2 = graphicContext.LabelY_1 + gapY;
            graphicContext.LabelY_3 = graphicContext.LabelY_2 + gapY;
            graphicContext.LabelY_4 = graphicContext.LabelY_3 + gapY;
            graphicContext.LabelY_5 = graphicContext.LabelY_4 + gapY;
            graphicContext.LabelY_6 = graphicContext.LabelY_5 + gapY;
            graphicContext.LabelY_7 = graphicContext.LabelY_6 + gapY;
            graphicContext.LabelY_8 = graphicContext.LabelY_7 + gapY;

            return pointsList;
        }

        private List<List<Point>> CoordinateTransformRs(Dictionary<string, List<Point>> dictionary)
        {
            List<List<Point>> pointsList = new List<List<Point>>();
            Point origin = new Point(drawingCanvas.Width / 2, drawingCanvas.Height);
            foreach (var item in dictionary)
            {
                foreach (var point in item.Value)
                {
                    if ((point.Y) > yMax)
                        yMax = point.Y;
                }
            }
            yMin = 0;
            xMin = dictionary.First().Value[0].X;
            gapX = (dictionary.First().Value[dictionary.First().Value.Count - 1].X - dictionary.First().Value[0].X) / 6;
            gapY = yMax / 8;
            double yScale = origin.Y / yMax;
            double xScale = drawingCanvas.Width / (dictionary.First().Value[dictionary.First().Value.Count - 1].X - dictionary.First().Value[0].X);
            foreach (var item in dictionary)
            {
                List<Point> pointsTemp = new List<Point>();
                foreach (var point in item.Value)
                {
                    pointsTemp.Add(new Point(point.X * xScale - dictionary.First().Value[0].X * xScale, -point.Y * yScale + origin.Y));
                }
                pointsList.Add(pointsTemp);
            }            

            graphicContext.LabelY_0 = yMin;
            graphicContext.LabelY_1 = graphicContext.LabelY_0 + gapY;
            graphicContext.LabelY_2 = graphicContext.LabelY_1 + gapY;
            graphicContext.LabelY_3 = graphicContext.LabelY_2 + gapY;
            graphicContext.LabelY_4 = graphicContext.LabelY_3 + gapY;
            graphicContext.LabelY_5 = graphicContext.LabelY_4 + gapY;
            graphicContext.LabelY_6 = graphicContext.LabelY_5 + gapY;
            graphicContext.LabelY_7 = graphicContext.LabelY_6 + gapY;
            graphicContext.LabelY_8 = graphicContext.LabelY_7 + gapY;

            return pointsList;
        }


        private List<List<Point>> CoordinateTransformError(Dictionary<string, List<Point>> dictionary, Point origin)
        {
            List<List<Point>> pointsList = new List<List<Point>>();
            yMax = 0;
            foreach (var item in dictionary)
            {
                foreach (var point in item.Value)
                {
                    if (Math.Abs(point.Y) >= yMax)
                    {
                        yMax = Math.Abs(point.Y);
                    }
                }
            }
            yMin = 0;
            xMin = dictionary.First().Value[0].X;
            gapX = (dictionary.First().Value[dictionary.First().Value.Count - 1].X - dictionary.First().Value[0].X) * 1000000 / 6;//坐标轴相关
            gapY = yMax / 4;//坐标轴相关
            double yScale = origin.Y / yMax;
            double xScale = drawingCanvas.Width / (dictionary.First().Value[dictionary.First().Value.Count - 1].X - dictionary.First().Value[0].X);            
            foreach (var item in dictionary)
            {
                List<Point> pointsTemp = new List<Point>();
                foreach (var point in item.Value)
                {
                    pointsTemp.Add(new Point(point.X * xScale - dictionary.First().Value[0].X * xScale, -point.Y * yScale + origin.Y));
                }
                pointsList.Add(pointsTemp);
            }

            graphicContext.LabelY_4 = yMin;            
            graphicContext.LabelY_3 = graphicContext.LabelY_4 - gapY;
            graphicContext.LabelY_2 = graphicContext.LabelY_3 - gapY;
            graphicContext.LabelY_1 = graphicContext.LabelY_2 - gapY;
            graphicContext.LabelY_0 = graphicContext.LabelY_1 - gapY;
            graphicContext.LabelY_5 = graphicContext.LabelY_4 + gapY;
            graphicContext.LabelY_6 = graphicContext.LabelY_5 + gapY;
            graphicContext.LabelY_7 = graphicContext.LabelY_6 + gapY;
            graphicContext.LabelY_8 = graphicContext.LabelY_7 + gapY;

            return pointsList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string url;//将要保存文件的路径

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPG格式(*.jpg)|*.jpg|JPEG格式(*.jpeg)|*.jpg|PNG格式(*.png)|*.png";
            saveFileDialog.InitialDirectory = "C:\\";
            saveFileDialog.ShowDialog();
            url = saveFileDialog.FileName;

            SavePic savePic = new SavePic();
            var control = test as Control;
            savePic.SaveVisual(control, url);
        }
    }
}
