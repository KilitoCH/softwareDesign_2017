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

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            storyboard.Begin(this);
        }

        private void PaintButton_Click(object sender, RoutedEventArgs e)
        {
            BPSK_Sequence_Generate bpskSequenceGenerate = new BPSK_Sequence_Generate(Convert.ToDouble(context.FrequenceBpsk), 2);//利用TextBox中输入的参数获取一个序列
            BOC_Sequence_Generate bocSequenceGenerate = new BOC_Sequence_Generate(Convert.ToInt32(context.Alpha), Convert.ToInt32(context.Beta));//利用TextBox中输入的参数获取一个序列
            NewThread newThread = new NewThread();

            if (context.WhichMode)
            {
                if (context.TimeDomain)
                {
                    context.points = new List<Point>();
                    newThread.whichFigure = "bpskTimeDomain";
                }                    
                else if (context.Psd)
                {
                    context.points = bpskSequenceGenerate.GetPsdSequenceRealDb;//把计算得到的点序列存到context类中
                    newThread.whichFigure = "bpskPsd";
                }                    
                else if (context.Autocorrelation)
                {
                    context.points = bpskSequenceGenerate.GetAutocorrelationSequence;
                    newThread.whichFigure = "bpskAutocorrelation";
                }                    
                if ((context.points != null) && (context.points.Count != 0))
                {
                    newThread.points = context.points;
                    Thread thread = new Thread(new ThreadStart(newThread.StartThread));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
                else
                    MessageBox.Show("请先输入频率", "错误");
            }
            else
            {
                if (context.TimeDomain)
                {
                    context.points = new List<Point>();
                    newThread.whichFigure = "bocTimeDomain";
                }
                else if (context.Psd)
                {
                    context.points = bocSequenceGenerate.GetPsdSequenceRealDb;//把计算得到的点序列存到context类中
                    newThread.whichFigure = "bocPsd";
                }
                else if (context.Autocorrelation)
                {
                    context.points = bocSequenceGenerate.GetAutocorrelationSequence;
                    newThread.whichFigure = "bocAutocorrelation";
                }
                if ((context.points != null) && (context.points.Count != 0))
                {
                    newThread.points = context.points;
                    Thread thread = new Thread(new ThreadStart(newThread.StartThread));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
                else
                    MessageBox.Show("请先输入频率", "错误");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string url;//将要保存文件的路径

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPG格式(*.jpg)|*.jpg|JPEG格式(*.jpeg)|*.jpg|PNG格式(*.png)|*.png";
            saveFileDialog.InitialDirectory = "C:\\";
            saveFileDialog.ShowDialog();
            url = saveFileDialog.FileName;

            SavePic savePic = new SavePic();
            //savePic.SaveVisual(, url);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBox;
            if (control == null)
                return;
            control.SelectAll();
        }

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as TextBox;
            if (control == null)
                return;
            Keyboard.Focus(control);
            e.Handled = true;
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

        private void Psd_Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ViewPara_Button_Click(object sender, RoutedEventArgs e)
        {
            var thread = new Thread(threadStart);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void threadStart()
        {
            var viewPara = new ViewPara();
            viewPara.ShowDialog();
        }
    }    
}
