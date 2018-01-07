using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private double yMax = 0;//所有点的Y的最大值绝对值
        private double yMin = 0;//所有点的Y的最小值
        private double xMin = 0;//所有点的X的最小值
        private double gapX = 0;//坐标系的Y轴步进
        private double gapY = 0;//坐标系的X轴步进

        private bool zoomInFlag = false;
#endregion
        public Graphic(Dictionary<string,List<Point>> dictionary,string yLabel)
        {            
            this.dictionary = dictionary;            
            DataContext = graphicContext;

            InitializeComponent();

            Plot(yLabel);
        }

        #region 方法        
        /// <summary>
        /// 在更新帧之前对显示坐标文本框的位置动画赋值
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            posTextBlock.RenderTransform = translateTransform;//为显示坐标的文本框指定坐标变换
        }

        /// <summary>
        /// 仅适用于将db最小值设定为-100的序列的坐标变换，较特殊无一般性
        /// </summary>
        /// <param name="pointsReal"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private List<List<Point>> CoordinateTransformDb(List<List<Point>> list)
        {
            List<List<Point>> pointsList = new List<List<Point>>();//创建用于返回点集集合的实例
            Point origin = new Point(drawingCanvas.Width / 2, drawingCanvas.Height);//选定绘图的中点
            foreach (var item in list)//找出最大值
            {
                foreach (var point in item)
                {
                    if ((point.Y + 100) > yMax)//由于控件的Y为上小下大，所以需要进行一些特殊处理
                        yMax = point.Y + 100;
                }               
            }
            //以下四行为坐标系相关
            yMin = -100;
            xMin = list.First()[0].X / 1000000;
            gapX = (list.First()[list.First().Count - 1].X - list.First()[0].X) / 6000000;
            gapY = yMax / 8;
            //以下两行用于设定将图形扩充到整个画布的参数
            double yScale = origin.Y / yMax;
            double xScale = drawingCanvas.Width / (list.First()[list.First().Count - 1].X - list.First()[0].X);
            foreach (var item in list)//进行坐标变化以适应画布大小
            {
                List<Point> pointsTemp = new List<Point>();
                foreach (var point in item)
                {
                    pointsTemp.Add(new Point(point.X * xScale - list.First()[0].X * xScale, -(point.Y + 100) * yScale + origin.Y));
                }
                pointsList.Add(pointsTemp);
            }
            //设定Y坐标系并将数值格式化为小数点后两位
            graphicContext.LabelY_0 = yMin.ToString("f");
            graphicContext.LabelY_1 = (yMin + gapY).ToString("f");
            graphicContext.LabelY_2 = (yMin + 2 * gapY).ToString("f");
            graphicContext.LabelY_3 = (yMin + 3 * gapY).ToString("f");
            graphicContext.LabelY_4 = (yMin + 4 * gapY).ToString("f");
            graphicContext.LabelY_5 = (yMin + 5 * gapY).ToString("f");
            graphicContext.LabelY_6 = (yMin + 6 * gapY).ToString("f");
            graphicContext.LabelY_7 = (yMin + 7 * gapY).ToString("f");
            graphicContext.LabelY_8 = (yMin + 8 * gapY).ToString("f");

            return pointsList;
        }

        /// <summary>
        /// 适用于所有点的Y坐标均为正值的坐标变换
        /// </summary>
        /// <param name="list">要变换的序列</param>
        /// <returns></returns>
        private List<List<Point>> CoordinateTransformRs(List<List<Point>> list)
        {
            List<List<Point>> pointsList = new List<List<Point>>();
            Point origin = new Point(drawingCanvas.Width / 2, drawingCanvas.Height);
            if(flag != 4)//如果标志位不为为4则寻找y的最大值
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
            else//如果标志位为4则为码跟踪精度随信噪比变化的图像，将坐标轴最大值设为1
                yMax = 1;
            //以下四行为坐标轴相关
            yMin = 0;
            xMin = list.First()[0].X;
            gapX = (list.First()[list.First().Count - 1].X - list.First()[0].X) / 6;
            gapY = yMax / 8;
            //以下两行用于坐标变换
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
            //设定y坐标轴
            graphicContext.LabelY_0 = yMin.ToString("f");
            graphicContext.LabelY_1 = (yMin + gapY).ToString("f");
            graphicContext.LabelY_2 = (yMin + 2 * gapY).ToString("f");
            graphicContext.LabelY_3 = (yMin + 3 * gapY).ToString("f");
            graphicContext.LabelY_4 = (yMin + 4 * gapY).ToString("f");
            graphicContext.LabelY_5 = (yMin + 5 * gapY).ToString("f");
            graphicContext.LabelY_6 = (yMin + 6 * gapY).ToString("f");
            graphicContext.LabelY_7 = (yMin + 7 * gapY).ToString("f");
            graphicContext.LabelY_8 = (yMin + 8 * gapY).ToString("f");

            return pointsList;
        }

        /// <summary>
        /// 适用于Y坐标有正有负的坐标变换
        /// </summary>
        /// <param name="list">要变换的序列</param>
        /// <returns></returns>
        private List<List<Point>> CoordinateTransformError(List<List<Point>> list)
        {
            List<List<Point>> pointsList = new List<List<Point>>();//所有用法同上
            Point origin = new Point(drawingCanvas.Width, drawingCanvas.Height / 2);
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

            graphicContext.LabelY_4 = yMin.ToString("f");            
            graphicContext.LabelY_3 = (yMin - gapY).ToString("f");
            graphicContext.LabelY_2 = (yMin - 2 * gapY).ToString("f");
            graphicContext.LabelY_1 = (yMin - 3 * gapY).ToString("f");
            graphicContext.LabelY_0 = (yMin - 4 * gapY).ToString("f");
            graphicContext.LabelY_5 = (yMin + gapY).ToString("f");
            graphicContext.LabelY_6 = (yMin + 2 * gapY).ToString("f");
            graphicContext.LabelY_7 = (yMin + 3 * gapY).ToString("f");
            graphicContext.LabelY_8 = (yMin + 4 * gapY).ToString("f");

            return pointsList;
        }

        /// <summary>
        /// 保存图片到本地按钮的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            string url;//将要保存文件的路径

            SaveFileDialog saveFileDialog = new SaveFileDialog();//打开标准的Win32保存路径选择对话框
            saveFileDialog.Filter = "JPG格式(*.jpg)|*.jpg|JPEG格式(*.jpeg)|*.jpg|PNG格式(*.png)|*.png";//能选择的格式
            saveFileDialog.InitialDirectory = "C:\\";//设定初始的路径
            saveFileDialog.ShowDialog();
            url = saveFileDialog.FileName;//获取对话框返回的路径

            SavePic savePic = new SavePic();//创建新的保存图片实例
            var control = screenShot as Control;
            savePic.SaveVisual(control, url);//将控件按照指定路径保存为图片
        }

        /// <summary>
        /// 当listBox的选中项发生变化时的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Bold(listBox.SelectedIndex);//将选中的图像加粗显示
        }

        /// <summary>
        /// 将指定索引的图像加粗显示
        /// </summary>
        /// <param name="index"></param>
        private void Bold(int index)
        {
            List<Point> tempPoints = dictionary.ElementAt(index).Value;
            List<Visual> tempVisualList = new List<Visual>();
            Paint paint = new Paint();
            if (flag != 6)//若不是多径对码跟踪精度影响的图像，直接选中加粗
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
            else//若是，则因为每一个参数有两条曲线，应对颜色进行一些处理
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
            //重画图像
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
            if (yLabel.Equals("时域波形"))//这个if为将序列坐标变换后，生成visual对象，并添加显示，同时设定标志位，以保存本次所绘曲线的类型
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
            //重画图像
            drawingCanvas.RemoveAll();
            foreach (var visual in InitialVisualList)
            {
                drawingCanvas.AddVisual(visual);
            }
            RuntimeVisualList = InitialVisualList;//设定命中判断所用的visual集合

            for (int i = 0; i < dictionary.Count; i++)//添加listBox的项，以创建图例
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
            //设定x坐标
            graphicContext.LabelX_0 = xMin.ToString("f"); ;
            graphicContext.LabelX_1 = (xMin + gapX).ToString("f");
            graphicContext.LabelX_2 = (xMin + 2 * gapX).ToString("f");
            graphicContext.LabelX_3 = (xMin + 3 * gapX).ToString("f");
            graphicContext.LabelX_4 = (xMin + 4 * gapX).ToString("f");
            graphicContext.LabelX_5 = (xMin + 5 * gapX).ToString("f");
            graphicContext.LabelX_6 = (xMin + 6 * gapX).ToString("f");

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

            if (flag == 1)//这个if为根据标志位和新的序列重绘图像
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
            //重绘图像
            drawingCanvas.RemoveAll();
            foreach (var visual in tempVisualList)
            {
                drawingCanvas.AddVisual(visual);
            }
            RuntimeVisualList = tempVisualList;

            //重新设定x坐标轴
            graphicContext.LabelX_0 = xMin.ToString("f");//只保留小数点后两位
            graphicContext.LabelX_1 = (xMin + gapX).ToString("f");
            graphicContext.LabelX_2 = (xMin + 2 * gapX).ToString("f");
            graphicContext.LabelX_3 = (xMin + 3 * gapX).ToString("f");
            graphicContext.LabelX_4 = (xMin + 4 * gapX).ToString("f");
            graphicContext.LabelX_5 = (xMin + 5 * gapX).ToString("f");
            graphicContext.LabelX_6 = (xMin + 6 * gapX).ToString("f");

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
            RuntimeVisualList = InitialVisualList;//重新设定用于命中判断的visual集合
        }

        /// <summary>
        /// 当鼠标在画布上移动的时候，在鼠标旁边显示当前点的坐标位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point posWithCanvas = Mouse.GetPosition(drawingCanvas);//保存鼠标相对于画布的位置
            int? index = null;//命中的visual对象在集合中的索引
            HitTestResult hitTestResult = null;//命中判断的结果
            for (int i = 0; i < RuntimeVisualList.Count; i++)//进行命中判断
            {
                hitTestResult = VisualTreeHelper.HitTest(RuntimeVisualList[i], posWithCanvas);
                if (hitTestResult != null)//若命中则保存索引并跳出循环
                {
                    index = i;
                    break;
                }
                
            }
            if (index != null)
            {
                if (flag == 0)
                {
                    graphicContext.Position = "在时域图形中不支持坐标显示";//在时域图形中不支持坐标显示
                }
                else
                    graphicContext.Position = "(" + PosMapping(posWithCanvas).X.ToString("f") + "," + PosMapping(posWithCanvas).Y.ToString("f") + ")";//在坐标映射后显示鼠标位置曲线的坐标。只保留小数点后两位。
                Point posWithTextBlock = Mouse.GetPosition(posTextBlock);//控制文本框显示在鼠标右下角
                translateTransform.X += posWithTextBlock.X + 10;
                translateTransform.Y += posWithTextBlock.Y + 10;
            }
            else
            {
                graphicContext.Position = null;
            }            
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
            if (flag == 1)//根据不同的坐标变换情况进行坐标映射
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

        /// <summary>
        /// 当鼠标点击某区域时将其放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (flag == 0)
            {
                MessageBox.Show("时域图形中不支持放大","提示");
                return;
            }
            List<List<Point>> tempPointsList = new List<List<Point>>();
            List<List<Point>> pointsList = new List<List<Point>>();//此处的指代真实值

            foreach (var item in dictionary)
            {
                pointsList.Add(item.Value);
            }

            if (zoomIn.IsChecked == true)
            {
                if (!zoomInFlag)//如果还未放大，则在点击时将其放大。
                {
                    Point pos = Mouse.GetPosition(drawingCanvas);
                    double xPercentage = pos.X / drawingCanvas.Width;
                    if ((xPercentage >= 0.2) && (xPercentage <= 0.8))//若不在边缘位置，则放大固定大小
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
                    else if (xPercentage < 0.2)//若在边缘位置按照到边缘距离放大
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
                    zoomInFlag = true;//设定是否已放大的标志位为已放大
                }
                else//若已放大过则重新恢复为初始状态
                {
                    ReplotNormal();
                    zoomInFlag = false;//设定是否已放大的标志位为未放大
                }
            }
        }
        #endregion
    }
}
