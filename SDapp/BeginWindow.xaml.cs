using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Win32;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// BeginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BeginWindow : Window
    {
#region 字段
        private Storyboard storyboard = new Storyboard();
        private Context context = new Context();
#endregion
        public BeginWindow()
        {
            DataContext = context;
            InitializeComponent();
            InitializeAnimation();
        }

        private void Begin_Button_Click(object sender, RoutedEventArgs e)
        {            
            storyboard.Begin(this);
        }

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
            DoubleAnimation animationBackground = new DoubleAnimation(80, new Duration(TimeSpan.FromSeconds(1)));
            Storyboard.SetTargetName(animationBackground, backgroundField.Name);
            Storyboard.SetTargetProperty(animationBackground, new PropertyPath(HeightProperty));
            //图片的动画，缓慢消失
            DoubleAnimation animationImage = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(1)));
            Storyboard.SetTargetName(animationImage, image.Name);
            Storyboard.SetTargetProperty(animationImage, new PropertyPath(OpacityProperty));
            //输入控件的动画            
            DoubleAnimation aniBpskGrid = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(1)));
            aniBpskGrid.BeginTime = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetName(aniBpskGrid, controlGrid.Name);
            Storyboard.SetTargetProperty(aniBpskGrid, new PropertyPath(OpacityProperty));


            storyboard.Children.Add(animationBackground);
            storyboard.Children.Add(animationImage);
            storyboard.Children.Add(aniBpskGrid);
        }

        private void ViewPara_Button_Click(object sender, RoutedEventArgs e)
        {
            var thread = new Thread(ViewParathreadStart);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void ViewParathreadStart()
        {
            var viewPara = new ViewPara();
            viewPara.ShowDialog();
        }

        private void DrawGrapgicthreadStart()
        {
            var drawGraphic = new DrawGraphic();
            drawGraphic.ShowDialog();
        }
    }    
}
