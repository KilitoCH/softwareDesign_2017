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
        public Advance()
        {
            //从文件中读取预设参数
            FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream);
            string[] str = streamReader.ReadLine().Split(',');            
            
            InitializeComponent();

            bW.Text = str[0];
            foreBW.Text = str[1];
            psdCount.Text = str[2];
            acCount.Text = str[3];
        }

        private void Submmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Create, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(bW.Text + ',' + foreBW.Text + ',' + psdCount.Text + ',' + acCount.Text);
                streamWriter.Close();
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
                FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Create, FileAccess.Write);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write("30000000,24000000,3000,2000");
                streamWriter.Close();
                bW.Text = "30000000";
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
