using System;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Threading;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(OpenMainWindow);
            var mainThread = Thread.CurrentThread;
            var val = mainThread.ManagedThreadId;
            thread.SetApartmentState(ApartmentState.STA);
            //thread.Start();
            
            storyboard.Begin(this);
        }

        private void OpenMainWindow()
        {
            MainWindow mainWindow = new MainWindow();            
            mainWindow.ShowDialog();
        }

        /// <summary>
        /// 当按下欢迎界面的按钮时将触发的所有动画效果
        /// </summary>
        private void InitializeAnimation()
        {
            //背景的动画，高度逐渐减少
            DoubleAnimation animationBackground = new DoubleAnimation(80, new Duration(TimeSpan.FromSeconds(1)));
            Storyboard.SetTargetName(animationBackground, backgroundField.Name);
            Storyboard.SetTargetProperty(animationBackground, new PropertyPath(Rectangle.HeightProperty));
            //图片的动画，缓慢消失
            DoubleAnimation animationImage = new DoubleAnimation(0, new Duration(TimeSpan.FromSeconds(1)));
            Storyboard.SetTargetName(animationImage, image.Name);
            Storyboard.SetTargetProperty(animationImage, new PropertyPath(Image.OpacityProperty));
            //输入控件的动画
            DoubleAnimation animationBpskFreqTextBox = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(1)));
            animationBpskFreqTextBox.BeginTime = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetName(animationBpskFreqTextBox, bpskFreqTextBox.Name);
            Storyboard.SetTargetProperty(animationBpskFreqTextBox, new PropertyPath(TextBox.OpacityProperty));
            DoubleAnimation animationBpskFreqLabel = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(1)));
            animationBpskFreqLabel.BeginTime = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetName(animationBpskFreqLabel, bpskFreqLabel.Name);
            Storyboard.SetTargetProperty(animationBpskFreqLabel, new PropertyPath(Label.OpacityProperty));

            storyboard.Children.Add(animationBackground);
            storyboard.Children.Add(animationImage);
            storyboard.Children.Add(animationBpskFreqLabel);
            storyboard.Children.Add(animationBpskFreqTextBox);
        }
    }    
}
