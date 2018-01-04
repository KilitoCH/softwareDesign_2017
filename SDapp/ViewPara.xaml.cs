using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// ViewPara.xaml 的交互逻辑
    /// </summary>
    public partial class ViewPara : Window
    {
        private ObservableCollection<Param> paramList = new ObservableCollection<Param>();
        private ViewParaContext context = new ViewParaContext();

        public ViewPara()
        {
            DataContext = context;
            InitializeComponent();
            listView.ItemsSource = paramList;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name;
            if (context.FrequenceBpsk != 0)
            {
                name = String.Format(context.FrequenceBpsk + "MHzBPSK");
                if (context.nameList.Contains(name))
                {
                    MessageBox.Show("已添加过该BPSK参数", "提示");
                }
                else
                {
                    BPSK_Sequence_Generate bpskSequenceGenerate = new BPSK_Sequence_Generate(Convert.ToDouble(context.FrequenceBpsk));//利用TextBox中输入的参数获取一个序列
                    paramList.Add(new Param(name, bpskSequenceGenerate.GetPsdSequenceReal, null));
                    context.nameList.Add(name);
                }
            }
            if ((context.Alpha != 0) && (context.Beta != 0))
            {
                name = String.Format("BOC(" + context.Alpha + "," + context.Beta + ")");
                if (context.nameList.Contains(name))
                {
                    MessageBox.Show("已添加过该BOC参数", "提示");
                }
                else
                {
                    BOC_Sequence_Generate bocSequenceGenerate = new BOC_Sequence_Generate(Convert.ToInt32(context.Alpha), Convert.ToInt32(context.Beta));//利用TextBox中输入的参数获取一个序列
                    paramList.Add(new Param(name, bocSequenceGenerate.GetPsdSequenceReal, bocSequenceGenerate.GetAutocorrelationSequence));
                    context.nameList.Add(name);
                }
            }
            context.FrequenceBpsk = 0;
            context.Alpha = 0;
            context.Beta = 0;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            string url;//将要保存文件的路径

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPG格式(*.jpg)|*.jpg|JPEG格式(*.jpeg)|*.jpg|PNG格式(*.png)|*.png";
            saveFileDialog.InitialDirectory = "C:\\";
            saveFileDialog.ShowDialog();
            url = saveFileDialog.FileName;

            var control = listView as Control;
            SavePic savePic = new SavePic();
            savePic.SaveControl(control, url);
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                context.nameList.RemoveAt(listView.SelectedIndex);
                paramList.RemoveAt(listView.SelectedIndex);
            }
            catch (Exception)
            {
                MessageBox.Show("请先选择一项再点击删除按钮", "提示");
            }
                  
        }

        private void RemoveAll_Button_Click(object sender, RoutedEventArgs e)
        {
            context.nameList.Clear();
            paramList.Clear();
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

        private void Advance_Click(object sender, RoutedEventArgs e)
        {
            Advance advance = new Advance();
            advance.ShowDialog();
        }
    }
}
