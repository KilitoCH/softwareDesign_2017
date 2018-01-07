using System;
using System.Collections.ObjectModel;
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
        private ObservableCollection<Param> paramList = new ObservableCollection<Param>();//与listView数据绑定的集合
        private ViewParaContext context = new ViewParaContext();//与窗口进行数据绑定的实例

        public ViewPara()
        {
            DataContext = context;//进行窗口数据绑定
            InitializeComponent();
            listView.ItemsSource = paramList;//进行listView的数据绑定
        }

        /// <summary>
        /// 添加按钮的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
                        paramList.Add(new Param(name, bocSequenceGenerate.GetPsdSequenceReal, bocSequenceGenerate.GetAutocorrelationSequence));//添加到listView显示
                        context.nameList.Add(name);//在已添加列表中做出标记以免重复添加
                    }
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                context.FrequenceBpsk = 0;
                context.Alpha = 0;
                context.Beta = 0;
            }
        }

        /// <summary>
        /// 保存按钮的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            string url;//将要保存文件的路径

            SaveFileDialog saveFileDialog = new SaveFileDialog();//创建新的标准Win32保存路径选择按钮的实例
            saveFileDialog.Filter = "JPG格式(*.jpg)|*.jpg|JPEG格式(*.jpeg)|*.jpg|PNG格式(*.png)|*.png";
            saveFileDialog.InitialDirectory = "C:\\";
            saveFileDialog.ShowDialog();
            url = saveFileDialog.FileName;

            var control = listView as Control;
            SavePic savePic = new SavePic();
            savePic.SaveControl(control, url);//将控件按照指定的路径保存为图片
        }

        /// <summary>
        /// 移除按钮的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                context.nameList.RemoveAt(listView.SelectedIndex);//按照索引移除项
                paramList.RemoveAt(listView.SelectedIndex);
            }
            catch (Exception)
            {
                MessageBox.Show("请先选择一项再点击删除按钮", "提示");
            }
                  
        }

        /// <summary>
        /// 全部移除按钮的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAll_Button_Click(object sender, RoutedEventArgs e)
        {
            context.nameList.Clear();
            paramList.Clear();
        }

        /// <summary>
        /// 当文本框获得焦点时的事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var control = sender as TextBox;
            if (control == null)
                return;
            control.SelectAll();
        }

        /// <summary>
        /// 当鼠标左键按下时的文本框路由事件处理程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as TextBox;
            if (control == null)
                return;
            Keyboard.Focus(control);//设定键盘焦点
            e.Handled = true;
        }

        /// <summary>
        /// 打开首选项窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Advance_Click(object sender, RoutedEventArgs e)
        {
            Advance advance = new Advance();
            advance.ShowDialog();
        }
    }
}
