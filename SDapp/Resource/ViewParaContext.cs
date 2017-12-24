using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SoftwareDesign_2017
{
    class ViewParaContext : INotifyPropertyChanged
    {
        private double frequenceBpsk = 0;
        private int alpha = 0;
        private int beta = 0;

        public double FrequenceBpsk
        {
            get { return frequenceBpsk; }
            set
            {
                frequenceBpsk = value;
                OnPropertyChanged("FrequenceBpsk");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public int Alpha
        {
            get { return alpha; }
            set
            {
                alpha = value;
                OnPropertyChanged("Alpha");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public int Beta
        {
            get { return beta; }
            set
            {
                beta = value;
                OnPropertyChanged("Beta");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }


        #region 方法
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public List<String> nameList = new List<string>();
    }
}
