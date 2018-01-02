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
        #region 字段
        private GraphicContext graphicContext = new GraphicContext();
        private Dictionary<string, List<Point>> dictionary;
        private List<List<Point>> pointsList = new List<List<Point>>();//第一次原始绘图之后获取到的用于绘图的序列
        private List<Visual> InitialVisualList = new List<Visual>();//最初的一次画图所产生的visual对象，赋值后不更改用作重画原图用途
        private List<Visual> RuntimeVisualList = new List<Visual>();//每次对visual树做出改变时所调用的临时存储

        private TranslateTransform translateTransform = new TranslateTransform();

        private Color color = new Color();
        private int flag;
        private double yMax = 0;
        private double yMin = 0;
        private double xMin = 0;
        private double gapX = 0;
        private double gapY = 0;

        private bool zoomInFlag = false;
#endregion
        public Graphic(Dictionary<string,List<Point>> dictionary,string yLabel)
        {            
            this.dictionary = dictionary;            
            DataContext = graphicContext;

            InitializeComponent();

            Plot(yLabel);
        }

        /// <summary>
        /// 在更新帧之前对显示坐标文本框的位置动画赋值
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            posTextBlock.RenderTransform = translateTransform;
        }

        /// <summary>
        /// 将真实的PsdDb点阵根据显示空间的大小调整为动态的适合窗口的点阵
        /// </summary>
        /// <param name="pointsReal"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private List<List<Point>> CoordinateTransformDb(List<List<Point>> list)
        {
            List<List<Point>> pointsList = new List<List<Point>>();
            Point origin = new Point(drawingCanvas.Width / 2, drawingCanvas.Height);
            foreach (var item in list)
            {
                foreach (var point in item)
                {
                    if ((point.Y + 100) > yMax)
                        yMax = point.Y + 100;
                }               
            }
            yMin = -100;
            xMin = list.First()[0].X / 1000000;
            gapX = (list.First()[list.First().Count - 1].X - list.First()[0].X) / 6000000;
            gapY = yMax / 8;
            double yScale = origin.Y / yMax;
            double xScale = drawingCanvas.Width / (list.First()[list.First().Count - 1].X - list.First()[0].X);
            foreach (var item in list)
            {
                List<Point> pointsTemp = new List<Point>();
                foreach (var point in item)
                {
                    pointsTemp.Add(new Point(point.X * xScale - list.First()[0].X * xScale, -(point.Y + 100) * yScale + origin.Y));
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

        private List<List<Point>> CoordinateTransformRs(List<List<Point>> list)
        {
            List<List<Point>> pointsList = new List<List<Point>>();
            Point origin = new Point(drawingCanvas.Width / 2, drawingCanvas.Height);
            if(flag != 4)
            {
                foreach (var item in list)
                {
                    foreach (var point in item)
                    {
                        if ((point.Y) > yMax)
                            yMax = point.Y;
                    }
                }
            }
            else
                yMax = 0;
            yMin = 0;
            xMin = list.First()[0].X;
            gapX = (list.First()[list.First().Count - 1].X - list.First()[0].X) / 6;
            gapY = yMax / 8;
            double yScale = origin.Y / yMax;
            double xScale = drawingCanvas.Width / (list.First()[list.First().Count - 1].X - list.First()[0].X);
            foreach (var item in list)
            {
                List<Point> pointsTemp = new List<Point>();
                foreach (var point in item)
                {
                    pointsTemp.Add(new Point(point.X * xScale - list.First()[0].X * xScale, -point.Y * yScale + origin.Y));
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

        private List<List<Point>> CoordinateTransformError(List<List<Point>> list)
        {
            List<List<Point>> pointsList = new List<List<Point>>();
            Point origin = new Point(drawingCanvas.Width, drawingCanvas.Height / 2);
            yMax = 0;
            foreach (var item in list)
            {
                foreach (var point in item)
                {
                    if (Math.Abs(point.Y) >= yMax)
                    {
                        yMax = Math.Abs(point.Y);
                    }
                }
            }
            yMin = 0;
            xMin = list.First()[0].X;
            gapX = (list.First()[list.First().Count - 1].X - list.First()[0].X) / 6;//坐标轴相关
            gapY = yMax / 4;//坐标轴相关
            double yScale = origin.Y / yMax;
            double xScale = drawingCanvas.Width / (list.First()[list.First().Count - 1].X - list.First()[0].X);            
            foreach (var item in list)
            {
                List<Point> pointsTemp = new List<Point>();
                foreach (var point in item)
                {
                    pointsTemp.Add(new Point(point.X * xScale - list.First()[0].X * xScale, -point.Y * yScale + origin.Y));
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

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            string url;//将要保存文件的路径

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPG格式(*.jpg)|*.jpg|JPEG格式(*.jpeg)|*.jpg|PNG格式(*.png)|*.png";
            saveFileDialog.InitialDirectory = "C:\\";
            saveFileDialog.ShowDialog();
            url = saveFileDialog.FileName;

            SavePic savePic = new SavePic();
            var control = screenShot as Control;
            savePic.SaveVisual(control, url);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bold(listBox.SelectedIndex);
        }

        private void Bold(int index)
        {
            List<Point> tempPoints = dictionary.ElementAt(index).Value;
            List<Visual> tempVisualList = new List<Visual>();
            Paint paint = new Paint();
            if (flag != 6)
            {
                for (int i = 0; i < InitialVisualList.Count; i++)
                {
                    if (i != index)
                    {
                        tempVisualList.Add(InitialVisualList[i]);
                    }
                    else
                        tempVisualList.Add(paint.DrawVisual(pointsList[index], true, false, LineType.Line, new Pen(color.colors[i], 2.2)));
                }
            }
            else
            {
                for (int i = 0; i < InitialVisualList.Count; i++)
                {
                    if (i != index)
                    {
                        tempVisualList.Add(InitialVisualList[i]);
                    }
                    else
                        tempVisualList.Add(paint.DrawVisual(pointsList[index], true, false, LineType.Line, new Pen(color.colors[i / 2], 2.2)));
                }
            }
            drawingCanvas.RemoveAll();
            foreach (var item in tempVisualList)
            {
                drawingCanvas.AddVisual(item);
            }
        }

        /// <summary>
        /// 程序开始直接调用画出原始图像
        /// </summary>
        /// <param name="yLabel">要画的图的名称</param>
        /// <returns></returns>
        private bool Plot(string yLabel)
        {
            Paint paint = new Paint();
            List<List<Point>> formerList = new List<List<Point>>();

            foreach (var item in dictionary)
            {
                formerList.Add(item.Value);
            }
            if (yLabel.Equals("时域波形"))
            {
                flag = 0;
                pointsList = CoordinateTransformError(formerList);
                for (int i = 0; i < pointsList.Count; i++)
                {
                    InitialVisualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Line, new Pen(color.colors[i], 1.3)));
                }
                graphicContext.YLabel = yLabel;
                graphicContext.XLabel = "时间(ns)";
            }
            else if (yLabel.Equals("功率谱密度"))
            {
                flag = 1;
                pointsList = CoordinateTransformDb(formerList);
                for (int i = 0; i < pointsList.Count; i++)
                {
                    InitialVisualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1.3)));
                }
                graphicContext.YLabel = yLabel + "(dB)";
                graphicContext.XLabel = "频率(MHz)";
            }
            else if (yLabel.Equals("自相关函数"))
            {
                flag = 2;
                pointsList = CoordinateTransformRs(formerList);
                for (int i = 0; i < pointsList.Count; i++)
                {
                    InitialVisualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1.3)));
                }
                graphicContext.YLabel = yLabel;
                graphicContext.XLabel = "时延(microseconds)";
            }
            else if (yLabel.Equals("s曲线"))
            {
                flag = 3;
                pointsList = CoordinateTransformError(formerList);
                for (int i = 0; i < pointsList.Count; i++)
                {
                    InitialVisualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1.3)));
                }
                graphicContext.YLabel = yLabel;
                graphicContext.XLabel = "时延(microseconds)";
            }
            else if (yLabel.Equals("码跟踪精度(时延)"))
            {
                flag = 4;
                pointsList = CoordinateTransformRs(formerList);
                for (int i = 0; i < pointsList.Count; i++)
                {
                    InitialVisualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1.3)));
                }
                graphicContext.YLabel = yLabel;
                graphicContext.XLabel = "时延(microseconds)";
            }
            else if (yLabel.Equals("码跟踪精度(信噪比)"))
            {
                flag = 5;
                pointsList = CoordinateTransformRs(formerList);
                for (int i = 0; i < pointsList.Count; i++)
                {
                    InitialVisualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1.3)));
                }
                graphicContext.YLabel = yLabel;
                graphicContext.XLabel = "信噪比(dB)";
            }
            else if (yLabel.Equals("镜像多径引起的偏移误差"))
            {
                flag = 6;
                pointsList = CoordinateTransformError(formerList);
                for (int i = 0; i < pointsList.Count; i++)
                {
                    InitialVisualList.Add(paint.DrawVisual(pointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i / 2], 1.3)));
                }
                graphicContext.YLabel = yLabel;
                graphicContext.XLabel = "多径时延(ns)";
            }

            drawingCanvas.RemoveAll();
            foreach (var visual in InitialVisualList)
            {
                drawingCanvas.AddVisual(visual);
            }
            RuntimeVisualList = InitialVisualList;

            for (int i = 0; i < dictionary.Count; i++)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = dictionary.ElementAt(i).Key;
                if (flag != 6)
                {
                    listBoxItem.Foreground = color.colors[i];
                }
                else
                    listBoxItem.Foreground = color.colors[i / 2];
                Setter setter = new Setter(FontSizeProperty, 7, "listBoxItem");
                listBox.Items.Add(listBoxItem);
            }            

            graphicContext.LabelX_0 = xMin;
            graphicContext.LabelX_1 = graphicContext.LabelX_0 + gapX;
            graphicContext.LabelX_2 = graphicContext.LabelX_1 + gapX;
            graphicContext.LabelX_3 = graphicContext.LabelX_2 + gapX;
            graphicContext.LabelX_4 = graphicContext.LabelX_3 + gapX;
            graphicContext.LabelX_5 = graphicContext.LabelX_4 + gapX;
            graphicContext.LabelX_6 = graphicContext.LabelX_5 + gapX;

            return true;
        }

        /// <summary>
        /// 放大时画图函数
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private bool Plot(List<List<Point>> list)
        {
            Paint paint = new Paint();
            List<List<Point>> tempPointsList = new List<List<Point>>();
            List<Visual> tempVisualList = new List<Visual>();            

            if (flag == 1)
            {
                tempPointsList = CoordinateTransformDb(list);
                for (int i = 0; i < tempPointsList.Count; i++)
                {
                    tempVisualList.Add(paint.DrawVisual(tempPointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1.3)));
                }
            }
            else if (flag == 2)
            {
                tempPointsList = CoordinateTransformRs(list);
                for (int i = 0; i < tempPointsList.Count; i++)
                {
                    tempVisualList.Add(paint.DrawVisual(tempPointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1.3)));
                }
            }
            else if (flag == 3)
            {
                tempPointsList = CoordinateTransformError(list);
                for (int i = 0; i < tempPointsList.Count; i++)
                {
                    tempVisualList.Add(paint.DrawVisual(tempPointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1.3)));
                }
            }
            else if (flag == 4)
            {
                tempPointsList = CoordinateTransformRs(list);
                for (int i = 0; i < tempPointsList.Count; i++)
                {
                    tempVisualList.Add(paint.DrawVisual(tempPointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1.3)));
                }
            }
            else if (flag == 5)
            {
                tempPointsList = CoordinateTransformRs(list);
                for (int i = 0; i < tempPointsList.Count; i++)
                {
                    tempVisualList.Add(paint.DrawVisual(tempPointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i], 1.3)));
                }
            }
            else if (flag == 6)
            {
                tempPointsList = CoordinateTransformError(list);
                for (int i = 0; i < tempPointsList.Count; i++)
                {
                    tempVisualList.Add(paint.DrawVisual(tempPointsList[i], true, false, LineType.Bezier, new Pen(color.colors[i / 2], 1.3)));
                }
            }

            drawingCanvas.RemoveAll();
            foreach (var visual in tempVisualList)
            {
                drawingCanvas.AddVisual(visual);
            }
            RuntimeVisualList = tempVisualList;

            graphicContext.LabelX_0 = xMin;
            graphicContext.LabelX_1 = graphicContext.LabelX_0 + gapX;
            graphicContext.LabelX_2 = graphicContext.LabelX_1 + gapX;
            graphicContext.LabelX_3 = graphicContext.LabelX_2 + gapX;
            graphicContext.LabelX_4 = graphicContext.LabelX_3 + gapX;
            graphicContext.LabelX_5 = graphicContext.LabelX_4 + gapX;
            graphicContext.LabelX_6 = graphicContext.LabelX_5 + gapX;

            return true;
        }

        /// <summary>
        /// 按照无任何变换的visual对象重画
        /// </summary>
        private void ReplotNormal()
        {            
            drawingCanvas.RemoveAll();
            foreach (var item in InitialVisualList)
            {
                drawingCanvas.AddVisual(item);
            }
            RuntimeVisualList = InitialVisualList;
        }

        /// <summary>
        /// 当鼠标在画布上移动的时候，在鼠标旁边显示当前点的坐标位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point posWithCanvas = Mouse.GetPosition(drawingCanvas);
            int? index = null;
            HitTestResult hitTestResult = null;
            //(posWithCanvas.X / drawingCanvas.Width) * 
            for (int i = 0; i < RuntimeVisualList.Count; i++)
            {
                hitTestResult = VisualTreeHelper.HitTest(RuntimeVisualList[i], posWithCanvas);
                if (hitTestResult != null)
                {
                    index = i;
                    break;
                }
                
            }
            if (index != null)
            {
                graphicContext.Position = PosMapping(posWithCanvas).ToString();
                //Bold((int)index);这本来应该是鼠标放到线上就加粗的功能，由于暂时没法解决和listBox的冲突停用
            }
            else
            {
                graphicContext.Position = null;
                //ReplotNormal();
            }                

            Point posWithTextBlock = Mouse.GetPosition(posTextBlock);
            translateTransform.X += posWithTextBlock.X + 15;
            translateTransform.Y += posWithTextBlock.Y + 15;
        }

        /// <summary>
        /// 用于将鼠标捕捉的在画布中的位置转换为真实的坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Point PosMapping(Point point)
        {
            Point returnPoint = new Point();
            returnPoint.X = dictionary.ElementAt(0).Value[0].X + (dictionary.ElementAt(0).Value[dictionary.ElementAt(0).Value.Count - 1].X - dictionary.ElementAt(0).Value[0].X) * (point.X / drawingCanvas.Width);//利用鼠标位置相对画布的位置比例确定实际坐标
            if (flag == 1)
            {
                returnPoint.X /= 1000000;
                returnPoint.Y = yMin + ((yMax - 100) - yMin) * ((drawingCanvas.Height - point.Y) / drawingCanvas.Height);//利用鼠标位置相对画布的位置比例确定实际坐标
            }
            else if ((flag == 3) || (flag == 6))
            {
                returnPoint.Y = yMax * ((drawingCanvas.Height / 2 - point.Y) / (drawingCanvas.Height / 2));//利用鼠标位置相对画布的位置比例确定实际坐标
            }
            else
                returnPoint.Y = yMax * ((drawingCanvas.Height - point.Y) / drawingCanvas.Height);//利用鼠标位置相对画布的位置比例确定实际坐标
            return returnPoint;
        }

        /// <summary>
        /// 当鼠标离开画布区域的时候清空坐标显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            graphicContext.Position = null;
        }

        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<List<Point>> tempPointsList = new List<List<Point>>();
            List<List<Point>> pointsList = new List<List<Point>>();//此处的指代真实值

            foreach (var item in dictionary)
            {
                pointsList.Add(item.Value);
            }

            if (zoomIn.IsChecked == true)
            {
                if (!zoomInFlag)
                {
                    Point pos = Mouse.GetPosition(drawingCanvas);
                    double xPercentage = pos.X / drawingCanvas.Width;
                    if ((xPercentage >= 0.2) && (xPercentage <= 0.8))
                    {
                        int val1 = (int)(0.2 * pointsList[0].Count);
                        int val2 = (int)(xPercentage * pointsList[0].Count);
                        foreach (var item in pointsList)
                        {
                            List<Point> tempPoints = new List<Point>();
                            for (int i = val2 - val1; i < val2 + val1; i++)
                            {
                                tempPoints.Add(item[i]);
                            }
                            tempPointsList.Add(tempPoints);
                        }
                    }
                    else if (xPercentage < 0.2)
                    {
                        int val1 = (int)(xPercentage * pointsList[0].Count);
                        foreach (var item in pointsList)
                        {
                            List<Point> tempPoints = new List<Point>();
                            for (int i = 0; i < 2 * val1; i++)
                            {
                                tempPoints.Add(item[i]);
                            }
                            tempPointsList.Add(tempPoints);
                        }
                    }
                    else
                    {
                        int val1 = pointsList[0].Count - (int)(xPercentage * pointsList[0].Count);
                        foreach (var item in pointsList)
                        {
                            List<Point> tempPoints = new List<Point>();
                            for (int i = pointsList[0].Count - 2 * val1; i < pointsList[0].Count; i++)
                            {
                                tempPoints.Add(item[i]);
                            }
                            tempPointsList.Add(tempPoints);
                        }
                    }
                    Plot(tempPointsList);
                    zoomInFlag = true;
                }
                else
                {
                    ReplotNormal();
                    zoomInFlag = false;
                }
            }
        }
    }
}
