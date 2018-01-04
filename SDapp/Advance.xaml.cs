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
using System.IO;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// Advance.xaml 的交互逻辑
    /// </summary>
    public partial class Advance : Window
    {
        /// <summary>
        /// 初始化类，并对窗口的文本框赋予初值
        /// </summary>
        public Advance()
        {
            //从文件中读取预设参数
            FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Open, FileAccess.Read);//打开参数文件并将其读入文件流
            StreamReader streamReader = new StreamReader(fileStream);//创建新的流读取实例
            string[] str = streamReader.ReadLine().Split(',');//读取文件内容并按照','分隔以获取参数
            
            InitializeComponent();

            bW.Text = str[0];//以下四行为将参数读取到本类的私有字段中
            foreBW.Text = str[1];
            psdCount.Text = str[2];
            acCount.Text = str[3];
        }

        /// <summary>
        /// 提交按钮的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Create, FileAccess.Write);//直接覆盖原来的文件以写入新的参数
                StreamWriter streamWriter = new StreamWriter(fileStream);//创建新的流写入实例
                streamWriter.Write(bW.Text + ',' + foreBW.Text + ',' + psdCount.Text + ',' + acCount.Text);//将参数格式化为字符串后写入
                streamWriter.Close();//关闭写入流以保存
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Create, FileAccess.Write);//直接覆盖原来的文件以写入新的参数
                StreamWriter streamWriter = new StreamWriter(fileStream);//创建新的流写入实例
                streamWriter.Write("30000000,24000000,3000,2000");//按照预设参数重置文件内容
                streamWriter.Close();//关闭写入流以保存
                bW.Text = "30000000";//以下四行为根据预设内容刷新窗口上的显示值
                foreBW.Text = "24000000";
                psdCount.Text = "3000";
                acCount.Text = "2000";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
