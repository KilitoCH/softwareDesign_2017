using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            InitializeComponent();
        }

#region tabItem1的事件
        //选项卡1中的文本框的内容发生改变时触发的事件
        private void tabItem1_TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            var control = sender as TextBox;
            Sin_Sequence_Generate sin_Sequence_Generate = new Sin_Sequence_Generate();
            try
            {
                this.context.pointsForTabItem1 = sin_Sequence_Generate.Sin_Sequence(Convert.ToDouble(control.Text));
            }
            catch (Exception)
            {
                if (control.Text == "请输入相关频率") ;
                else
                    MessageBox.Show("输入浮点数");
            }            
        }

        //选项卡1中的提交按钮（tabItem1_Button1）被点击时触发的事件
        private void tabItem1_Button1_Click(object sender, RoutedEventArgs e)
        {
            Paint paint = new Paint();
            if (context.pointsForTabItem1 != null)
            {
                context.visualForTabItem1 = paint.DrawVisual(this.context.pointsForTabItem1, true, false, Type.Line);
                tabItem1_canvas.RemoveAll();
                tabItem1_canvas.AddVisual(context.visualForTabItem1);
            }
            else
                MessageBox.Show("请先输入参数再画图", "提示");
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
            SavePic savePic = new SavePic();
            savePic.SaveVisual(context.visualForTabItem1);
        }
        #endregion

    }
}
