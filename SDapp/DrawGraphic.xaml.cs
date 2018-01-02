using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// DrawGraphic.xaml 的交互逻辑
    /// </summary>
    public partial class DrawGraphic : Window
    {
        private Context context = new Context();
        private int? flagTab1;
        private int? flagTab2;

        public DrawGraphic()
        {
            DataContext = context;
            InitializeComponent();
        }

        private void Submmit_Button_Click(object sender, RoutedEventArgs e)
        {
            if (context.dictionaryBpskBoc.Count != 0)
            {
                NewThread newThread = new NewThread();
                switch (flagTab1)
                {
                    case 0:
                        newThread.WhichMode = "时域波形";
                        break;
                    case 1:
                        newThread.WhichMode = "功率谱密度";
                        break;
                    case 2:
                        newThread.WhichMode = "自相关函数";
                        break;
                }
                foreach (var item in context.dictionaryBpskBoc)
                {
                    newThread.dictionary.Add(item.Key, item.Value);
                }
                Thread thread = new Thread(new ThreadStart(newThread.StartThread));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                RemoveAll_Button_Click(null, null);//清除表格和存储的所有内容
                flagTab1 = null;//重置标志位
            }
            else
                MessageBox.Show("请先添加至少一个波形", "提示");
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                context.dictionaryBpskBoc.Remove(listBox_Tab1.SelectedItem as string);
                listBox_Tab1.Items.RemoveAt(listBox_Tab1.Items.IndexOf(listBox_Tab1.SelectedItem));
            }
            catch (Exception)
            {
                MessageBox.Show("请先选择一项再点击删除按钮", "提示");
            }
        }

        private void RemoveAll_Button_Click(object sender, RoutedEventArgs e)
        {
            listBox_Tab1.Items.Clear();
            context.dictionaryBpskBoc.Clear();
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            string name = null;
            try
            {
                if (context.WhichMode)
                {
                    if (context.dictionaryBpskBoc.Count == 0)
                    {
                        BPSK_Sequence_Generate bpsk_Sequence_Generate = new BPSK_Sequence_Generate(Convert.ToDouble(context.FrequenceBpsk));
                        if (context.TimeDomain)
                        {
                            flagTab1 = 0;
                            name = context.FrequenceBpsk + "MHz" + "BPSK" + "时域波形";
                            context.dictionaryBpskBoc.Add(name, bpsk_Sequence_Generate.GetBpskSequence);
                        }
                        else if (context.Psd)
                        {
                            flagTab1 = 1;
                            name = context.FrequenceBpsk + "MHz" + "BPSK" + "谱密度";
                            context.dictionaryBpskBoc.Add(name, bpsk_Sequence_Generate.GetPsdSequenceRealDb);
                        }
                        else if (context.Autocorrelation)
                        {
                            flagTab1 = 2;
                            name = context.FrequenceBpsk + "MHz" + "BPSK" + "自相关";
                            context.dictionaryBpskBoc.Add(name, bpsk_Sequence_Generate.GetAutocorrelationSequence);
                        }
                        listBox_Tab1.Items.Add(name);
                    }
                    else if (context.dictionaryBpskBoc.Count < 5)
                    {
                        BPSK_Sequence_Generate bpsk_Sequence_Generate = new BPSK_Sequence_Generate(Convert.ToDouble(context.FrequenceBpsk));
                        if (context.TimeDomain && (flagTab1 == 0))
                        {
                            throw new Exception("观察时域波形时只可添加一个");
                        }
                        else if (context.Psd && (flagTab1 == 1))
                        {
                            name = context.FrequenceBpsk + "MHz" + "BPSK" + "谱密度";
                            context.dictionaryBpskBoc.Add(name, bpsk_Sequence_Generate.GetPsdSequenceRealDb);
                            listBox_Tab1.Items.Add(name);
                        }
                        else if (context.Autocorrelation && (flagTab1 == 2))
                        {
                            name = context.FrequenceBpsk + "MHz" + "BPSK" + "自相关";
                            context.dictionaryBpskBoc.Add(name, bpsk_Sequence_Generate.GetAutocorrelationSequence);
                            listBox_Tab1.Items.Add(name);
                        }
                        else
                            throw new FormatException("请选择和第一次相同类别的波形！");
                    }
                    else
                        throw new IndexOutOfRangeException("最多添加五项！");
                }
                else
                {
                    if (context.dictionaryBpskBoc.Count == 0)
                    {
                        BOC_Sequence_Generate boc_Sequence_Generate = new BOC_Sequence_Generate(Convert.ToInt32(context.Alpha), Convert.ToInt32(context.Beta));
                        if (context.TimeDomain)
                        {
                            flagTab1 = 0;
                            name = "BOC" + "(" + context.Alpha + "," + context.Beta + ")" + "时域波形";
                            context.dictionaryBpskBoc.Add(name, boc_Sequence_Generate.GetBocSequence);
                            //添加序列
                        }
                        else if (context.Psd)
                        {
                            flagTab1 = 1;
                            name = "BOC" + "(" + context.Alpha + "," + context.Beta + ")" + "谱密度";
                            context.dictionaryBpskBoc.Add(name, boc_Sequence_Generate.GetPsdSequenceRealDb);
                        }
                        else if (context.Autocorrelation)
                        {
                            flagTab1 = 2;
                            name = "BOC" + "(" + context.Alpha + "," + context.Beta + ")" + "自相关";
                            context.dictionaryBpskBoc.Add(name, boc_Sequence_Generate.GetAutocorrelationSequence);
                        }
                        listBox_Tab1.Items.Add(name);
                    }
                    else if (context.dictionaryBpskBoc.Count < 5)
                    {
                        BOC_Sequence_Generate boc_Sequence_Generate = new BOC_Sequence_Generate(Convert.ToInt32(context.Alpha), Convert.ToInt32(context.Beta));
                        if (context.TimeDomain && (flagTab1 == 0))
                        {
                            throw new Exception("观察时域波形时只可添加一个");
                        }
                        else if (context.Psd && (flagTab1 == 1))
                        {
                            name = "BOC" + "(" + context.Alpha + "," + context.Beta + ")" + "谱密度";
                            context.dictionaryBpskBoc.Add(name, boc_Sequence_Generate.GetPsdSequenceRealDb);
                            listBox_Tab1.Items.Add(name);
                        }
                        else if (context.Autocorrelation && (flagTab1 == 2))
                        {
                            name = "BOC" + "(" + context.Alpha + "," + context.Beta + ")" + "自相关";
                            context.dictionaryBpskBoc.Add(name, boc_Sequence_Generate.GetAutocorrelationSequence);
                            listBox_Tab1.Items.Add(name);
                        }
                        else
                            throw new FormatException("请选择和第一次相同类别的波形！");
                    }
                    else
                        throw new IndexOutOfRangeException("最多添加五项！");
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("已添加该项", "提示");
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
            finally
            {
                //重置输入的参数
                context.FrequenceBpsk = 0;
                context.Alpha = 0;
                context.Beta = 0;
            }
        }

        private void Add2_Button_Click(object sender, RoutedEventArgs e)
        {
            string name = null;
            try
            {
                if (context.SLine)
                {
                    int delta;
                    switch (comboBox_SLine.SelectedIndex)
                    {
                        case 0:
                            delta = 20;
                            break;
                        case 1:
                            delta = 30;
                            break;
                        case 2:
                            delta = 40;
                            break;
                        case 3:
                            delta = 50;
                            break;
                        case 4:
                            delta = 60;
                            break;
                        default:
                            delta = 20;
                            break;
                    }
                    if (context.WhichMode_2)
                    {
                        if (context.dictionaryError.Count == 0)
                        {
                            BPSK_Sequence_Generate bpsk_Sequence_Generate = new BPSK_Sequence_Generate(Convert.ToDouble(context.FrequenceBpsk));
                            Error error = new Error();
                            flagTab2 = 0;
                            name = context.FrequenceBpsk + "MHz" + "BPSK" + delta + "ns";
                            context.dictionaryError.Add(name, error.Varone(bpsk_Sequence_Generate.GetAutocorrelationSequence, delta));
                            listBox_Tab2.Items.Add(name);
                        }
                        else if (context.dictionaryError.Count < 5)
                        {
                            BPSK_Sequence_Generate bpsk_Sequence_Generate = new BPSK_Sequence_Generate(Convert.ToDouble(context.FrequenceBpsk));
                            Error error = new Error();
                            if (flagTab2 == 0)
                            {
                                name = context.FrequenceBpsk + "MHz" + "BPSK" + delta + "ns";
                                context.dictionaryError.Add(name, error.Varone(bpsk_Sequence_Generate.GetAutocorrelationSequence, delta));
                                listBox_Tab2.Items.Add(name);
                            }
                            else
                                throw new FormatException("请选择和第一次相同类别的波形！");
                        }
                        else
                            throw new IndexOutOfRangeException("最多添加五项！");
                    }
                    else
                    {
                        if (context.dictionaryError.Count == 0)
                        {
                            BOC_Sequence_Generate boc_Sequence_Generate = new BOC_Sequence_Generate(Convert.ToInt32(context.Alpha), Convert.ToInt32(context.Beta));
                            Error error = new Error();
                            flagTab2 = 0;
                            name = "BOC" + "(" + context.Alpha + "," + context.Beta + ")" + delta + "ns";
                            context.dictionaryError.Add(name, error.Varone(boc_Sequence_Generate.GetAutocorrelationSequence, delta));
                            listBox_Tab2.Items.Add(name);
                        }
                        else if (context.dictionaryError.Count < 5)
                        {
                            BOC_Sequence_Generate boc_Sequence_Generate = new BOC_Sequence_Generate(Convert.ToInt32(context.Alpha), Convert.ToInt32(context.Beta));
                            Error error = new Error();
                            if (flagTab2 == 0)
                            {
                                name = "BOC" + "(" + context.Alpha + "," + context.Beta + ")" + delta + "ns";
                                context.dictionaryError.Add(name, error.Varone(boc_Sequence_Generate.GetAutocorrelationSequence, delta));
                                listBox_Tab2.Items.Add(name);
                            }
                            else
                                throw new FormatException("请选择和第一次相同类别的波形！");
                        }
                        else
                            throw new IndexOutOfRangeException("最多添加五项！");
                    }
                }
                else if (context.Delay)
                {
                    if (context.WhichMode_2)
                    {
                        if (context.dictionaryError.Count == 0)
                        {
                            BPSK_Sequence_Generate bpsk_Sequence_Generate = new BPSK_Sequence_Generate(Convert.ToDouble(context.FrequenceBpsk));
                            Error error = new Error();
                            flagTab2 = 1;
                            name = context.FrequenceBpsk + "MHz" + "BPSK" + "码跟踪误差";
                            context.dictionaryError.Add(name, error.SigmaDeltaSequenceGenerate(bpsk_Sequence_Generate.GetPsdSequenceReal));
                            listBox_Tab2.Items.Add(name);
                        }
                        else if (context.dictionaryError.Count < 5)
                        {
                            BPSK_Sequence_Generate bpsk_Sequence_Generate = new BPSK_Sequence_Generate(Convert.ToDouble(context.FrequenceBpsk));
                            Error error = new Error();
                            if (flagTab2 == 1)
                            {
                                name = context.FrequenceBpsk + "MHz" + "BPSK" + "码跟踪误差";
                                context.dictionaryError.Add(name, error.SigmaDeltaSequenceGenerate(bpsk_Sequence_Generate.GetPsdSequenceReal));
                                listBox_Tab2.Items.Add(name);
                            }
                            else
                                throw new FormatException("请选择和第一次相同类别的波形！");
                        }
                        else
                            throw new IndexOutOfRangeException("最多添加五项！");
                    }
                    else
                    {
                        if (context.dictionaryError.Count == 0)
                        {
                            BOC_Sequence_Generate boc_Sequence_Generate = new BOC_Sequence_Generate(Convert.ToInt32(context.Alpha), Convert.ToInt32(context.Beta));
                            Error error = new Error();
                            flagTab2 = 1;
                            name = "BOC" + "(" + context.Alpha + "," + context.Beta + ")" + "码跟踪误差";
                            context.dictionaryError.Add(name, error.SigmaDeltaSequenceGenerate(boc_Sequence_Generate.GetPsdSequenceReal));
                            listBox_Tab2.Items.Add(name);
                        }
                        else if (context.dictionaryError.Count < 5)
                        {
                            BOC_Sequence_Generate boc_Sequence_Generate = new BOC_Sequence_Generate(Convert.ToInt32(context.Alpha), Convert.ToInt32(context.Beta));
                            Error error = new Error();
                            if (flagTab2 == 1)
                            {
                                name = "BOC" + "(" + context.Alpha + "," + context.Beta + ")" + "码跟踪误差";
                                context.dictionaryError.Add(name, error.SigmaDeltaSequenceGenerate(boc_Sequence_Generate.GetPsdSequenceReal));
                                listBox_Tab2.Items.Add(name);
                            }
                            else
                                throw new FormatException("请选择和第一次相同类别的波形！");
                        }
                        else
                            throw new IndexOutOfRangeException("最多添加五项！");
                    }
                }
                else if(context.CN)
                {
                    BPSK_Sequence_Generate bpsk_Sequence_Generate;
                    BOC_Sequence_Generate boc_Sequence_Generate;
                    Error error = new Error();
                    List<Point> tempPoints = new List<Point>();

                    switch (comboBox_DB.SelectedIndex)
                    {
                        case 0:
                            bpsk_Sequence_Generate = new BPSK_Sequence_Generate(1.023);
                            name = "1.023MHz" + "PSK-R";
                            tempPoints = error.SigmaCNDbSequenceGenerate(bpsk_Sequence_Generate.GetPsdSequenceReal, "BPSK");
                            break;
                        case 1:
                            bpsk_Sequence_Generate = new BPSK_Sequence_Generate(10.23);
                            name = "10.23MHz" + "PSK-R";
                            tempPoints = error.SigmaCNDbSequenceGenerate(bpsk_Sequence_Generate.GetPsdSequenceReal, "BPSK");
                            break;
                        case 2:
                            boc_Sequence_Generate = new BOC_Sequence_Generate(5, 2);
                            name = "BOC" + "(5,2)";
                            tempPoints = error.SigmaCNDbSequenceGenerate(boc_Sequence_Generate.GetPsdSequenceReal, "BOC(5,2)");
                            break;
                        case 3:
                            boc_Sequence_Generate = new BOC_Sequence_Generate(8, 4);
                            name = "BOC" + "(8,4)";
                            tempPoints = error.SigmaCNDbSequenceGenerate(boc_Sequence_Generate.GetPsdSequenceReal, "BOC(8,4)");
                            break;
                        case 4:
                            boc_Sequence_Generate = new BOC_Sequence_Generate(10, 5);
                            name = "BOC" + "(10,5)";
                            tempPoints = error.SigmaCNDbSequenceGenerate(boc_Sequence_Generate.GetPsdSequenceReal, "BOC(10,5)");
                            break;
                    }
                    if (context.dictionaryError.Count == 0)
                    {
                        flagTab2 = 2;
                        context.dictionaryError.Add(name, tempPoints);
                        listBox_Tab2.Items.Add(name);
                    }
                    else if(context.dictionaryError.Count < 5)
                    {
                        if (flagTab2 == 2)
                        {
                            context.dictionaryError.Add(name, tempPoints);
                            listBox_Tab2.Items.Add(name);
                        }
                        else
                            throw new FormatException("请选择和第一次相同类别的波形！");
                    }
                    else
                        throw new IndexOutOfRangeException("最多添加五项！");
                }
                else if (context.MultiPath)
                {
                    BPSK_Sequence_Generate bpsk_Sequence_Generate;
                    BOC_Sequence_Generate boc_Sequence_Generate;
                    Error error = new Error();
                    List<Point> tempPoints_1 = new List<Point>();
                    List<Point> tempPoints_2 = new List<Point>();

                    switch (comboBox_MultiPath.SelectedIndex)
                    {
                        case 0:
                            bpsk_Sequence_Generate = new BPSK_Sequence_Generate(1.023);
                            name = "1.023MHz" + "PSK-R";
                            if(error.MultiPathSequenceGenerate(bpsk_Sequence_Generate.GetPsdSequenceReal,out tempPoints_1,out tempPoints_2, "BPSK"))
                                break;
                            else
                                throw new Exception("Error");
                        case 1:
                            bpsk_Sequence_Generate = new BPSK_Sequence_Generate(10.23);
                            name = "10.23MHz" + "PSK-R";
                            if (error.MultiPathSequenceGenerate(bpsk_Sequence_Generate.GetPsdSequenceReal, out tempPoints_1, out tempPoints_2, "BPSK"))
                                break;
                            else
                                throw new Exception("Error");
                        case 2:
                            boc_Sequence_Generate = new BOC_Sequence_Generate(5, 2);
                            name = "BOC" + "(5,2)";
                            if (error.MultiPathSequenceGenerate(boc_Sequence_Generate.GetPsdSequenceReal, out tempPoints_1, out tempPoints_2, "BPSK"))
                                break;
                            else
                                throw new Exception("Error");
                        case 3:
                            boc_Sequence_Generate = new BOC_Sequence_Generate(8, 4);
                            name = "BOC" + "(8,4)";
                            if (error.MultiPathSequenceGenerate(boc_Sequence_Generate.GetPsdSequenceReal, out tempPoints_1, out tempPoints_2, "BPSK"))
                                break;
                            else
                                throw new Exception("Error");
                        case 4:
                            boc_Sequence_Generate = new BOC_Sequence_Generate(10, 5);
                            name = "BOC" + "(10,5)";
                            if (error.MultiPathSequenceGenerate(boc_Sequence_Generate.GetPsdSequenceReal, out tempPoints_1, out tempPoints_2, "BPSK"))
                                break;
                            else
                                throw new Exception("Error");
                    }
                    if (context.dictionaryError.Count == 0)
                    {
                        flagTab2 = 3;
                        context.dictionaryError.Add(name + "Even", tempPoints_1);
                        context.dictionaryError.Add(name + "Odd", tempPoints_2);
                        listBox_Tab2.Items.Add(name);
                    }
                    else if (context.dictionaryError.Count < 10)
                    {
                        if (flagTab2 == 3)
                        {
                            context.dictionaryError.Add(name + "Even", tempPoints_1);
                            context.dictionaryError.Add(name + "Odd", tempPoints_2);
                            listBox_Tab2.Items.Add(name);
                        }
                        else
                            throw new FormatException("请选择和第一次相同类别的波形！");
                    }
                    else
                        throw new IndexOutOfRangeException("最多添加五项！");
                }
            }
            catch (ArgumentException)
            {
                MessageBox.Show("已添加该项", "提示");
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
        }

        private void Submmit2_Button_Click(object sender, RoutedEventArgs e)
        {
            if (context.dictionaryError.Count != 0)
            {
                NewThread newThread = new NewThread();
                switch (flagTab2)
                {
                    case 0:
                        newThread.WhichMode = "s曲线";
                        break;
                    case 1:
                        newThread.WhichMode = "码跟踪精度(时延)";
                        break;
                    case 2:
                        newThread.WhichMode = "码跟踪精度(信噪比)";
                        break;
                    case 3:
                        newThread.WhichMode = "镜像多径引起的偏移误差";
                        break;
                }
                foreach (var item in context.dictionaryError)
                {
                    newThread.dictionary.Add(item.Key, item.Value);
                }
                Thread thread = new Thread(new ThreadStart(newThread.StartThread));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                RemoveAll2_Button_Click(null, null);
                flagTab2 = null;
            }
            else
                MessageBox.Show("请先添加至少一个波形", "提示");
        }

        private void Remove2_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                context.dictionaryError.Remove(listBox_Tab2.SelectedItem as string);
                listBox_Tab2.Items.RemoveAt(listBox_Tab2.Items.IndexOf(listBox_Tab2.SelectedItem));
            }
            catch (Exception)
            {
                MessageBox.Show("请先选择一项再点击删除按钮", "提示");
            }
        }

        private void RemoveAll2_Button_Click(object sender, RoutedEventArgs e)
        {
            listBox_Tab2.Items.Clear();
            context.dictionaryError.Clear();
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
    }
}
