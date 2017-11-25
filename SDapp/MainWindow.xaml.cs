using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Context context = new Context();        

        public MainWindow()
        {
            DataContext = context;
            InitializeComponent();
        }        

#region tabItem1的事件        
        //选项卡1中的提交按钮（tabItem1_Button1）被点击时触发的事件
        private void tabItem1_Button1_Click(object sender, RoutedEventArgs e)
        {
            Paint paint = new Paint();
            BPSK_Sequence_Generate bpsk_Sequence_Generate = new BPSK_Sequence_Generate();
            try
            {
                context.pointsForTabItem1 = bpsk_Sequence_Generate.BPSK_PSD_Sequence(Convert.ToDouble(context.Frequence),context.FrequenceUnit);//利用TextBox中输入的频率值和频率的单位获取一个序列
                if (context.pointsForTabItem1 != null)//如果正确获取了该序列，则将其绘图
                {
                    context.visualForTabItem1 = paint.DrawVisual(this.context.pointsForTabItem1, true, false, Type.Bezier);
                    tabItem1_canvas.RemoveAll();
                    tabItem1_canvas.AddVisual(context.visualForTabItem1);
                }
                else
                    MessageBox.Show("请先输入参数再画图", "提示");
            }
            catch (Exception)
            {
                MessageBox.Show("输入浮点数");
            }            
        }

        //选项卡1中的文本框获取焦点时触发的事件
        private void tabItem1_TextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            tabItem1_TextBox1.SelectAll();
        }

        //当鼠标左键在选项卡1中的文本框中按下时触发的事件
        private void tabItem1_TextBox1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as TextBox;
            if (control == null)
                return;
            Keyboard.Focus(control);
            e.Handled = true;
        }

        //选项卡1中的保存图片按钮被按下时触发的事件
        private void tabItem1_Button2_Click(object sender, RoutedEventArgs e)
        {            
            string url;//将要保存文件的路径
            
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPG格式(*.jpg)|*.jpg|JPEG格式(*.jpeg)|*.jpg|PNG格式(*.png)|*.png";
            saveFileDialog.InitialDirectory = "C:\\";
            saveFileDialog.ShowDialog();
            url = saveFileDialog.FileName;

            SavePic savePic = new SavePic();
            savePic.SaveVisual(context.visualForTabItem1, url);
        }        
        #endregion
    }
}
