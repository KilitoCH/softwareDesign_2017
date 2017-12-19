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
    /// BeginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BeginWindow : Window
    {
        public BeginWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(OpenMainWindow);
            var mainThread = Thread.CurrentThread;
            var val = mainThread.ManagedThreadId;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void OpenMainWindow()
        {
            MainWindow mainWindow = new MainWindow();            
            mainWindow.ShowDialog();
        }
    }    
}
