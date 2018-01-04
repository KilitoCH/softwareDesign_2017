using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Threading;
using System.Windows;
using System.IO;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// BeginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BeginWindow : Window
    {
#region 字段
        private Storyboard storyboard = new Storyboard();//所有动画的故事板
        private Context context = new Context();//用于数据绑定的实例
#endregion
        public BeginWindow()
        {
            DataContext = context;
            if (!File.Exists(@"../../bandWidth.txt"))//打开程序之后，检查是否已有参数文件，若无则创建并初始化
            {
                FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Create, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.WriteLine("30000000,24000000,3000,2000");//初始化参数，从左到右依次为发射带宽，接收机前端带宽，psd序列点数，自相关序列点数
                streamWriter.Close();
                fileStream.Close();
            }            
            InitializeComponent();
            InitializeAnimation();
        }

        /// <summary>
        /// 开始使用按钮的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Begin_Button_Click(object sender, RoutedEventArgs e)
        {            
            storyboard.Begin(this);
        }

        /// <summary>
        /// 画图按钮的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaintButton_Click(object sender, RoutedEventArgs e)
        {
            var thread = new Thread(DrawGrapgicthreadStart);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        /// <summary>
        /// 当按下欢迎界面的按钮时将触发的所有动画效果
        /// </summary>
        private void InitializeAnimation()
        {
            //背景的动画，高度逐渐减少
            DoubleAnimation animationBackground = new DoubleAnimation(80, new Duration(TimeSpan.FromSeconds(1)));//创建新的线性动画，以下相同
            Storyboard.SetTargetName(animationBackground, backgroundField.Name);//设定动画的作用对象，以下相同
            Storyboard.SetTargetProperty(animationBackground, new PropertyPath(HeightProperty));//设定动画的作用属性
            //图片的动画，缓慢消失
            DoubleAnimation animationImage = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(1)));
            Storyboard.SetTargetName(animationImage, image.Name);
            Storyboard.SetTargetProperty(animationImage, new PropertyPath(OpacityProperty));
            //输入控件的动画，延时之后逐渐显现
            DoubleAnimation aniBpskGrid = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(1)));
            aniBpskGrid.BeginTime = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetName(aniBpskGrid, controlGrid.Name);
            Storyboard.SetTargetProperty(aniBpskGrid, new PropertyPath(OpacityProperty));

            storyboard.Children.Add(animationBackground);//将动画添加到故事板
            storyboard.Children.Add(animationImage);
            storyboard.Children.Add(aniBpskGrid);
        }

        /// <summary>
        /// 查看参数按钮的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewPara_Button_Click(object sender, RoutedEventArgs e)
        {
            var thread = new Thread(ViewParathreadStart);//创建新的线程
            thread.SetApartmentState(ApartmentState.STA);//设定线程的状态参数
            thread.Start();//启动新的线程
        }

        /// <summary>
        /// 打开查看参数窗口
        /// </summary>
        private void ViewParathreadStart()
        {
            var viewPara = new ViewPara();
            viewPara.ShowDialog();
        }

        /// <summary>
        /// 打开绘图窗口
        /// </summary>
        private void DrawGrapgicthreadStart()
        {
            var drawGraphic = new DrawGraphic();
            drawGraphic.ShowDialog();
        }
    }    
}
