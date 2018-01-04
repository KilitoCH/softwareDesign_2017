using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// 这个类存储了在主界面中画图的时候产生的点阵序列和Visual对象
    /// </summary>
    class Context:INotifyPropertyChanged
    {
#region 字段
        public List<Point> points = new List<Point>();//BpskPSD点阵
        public Dictionary<string, List<Point>> dictionaryBpskBoc = new Dictionary<string, List<Point>>();
        public Dictionary<string, List<Point>> dictionaryError = new Dictionary<string, List<Point>>();

        private double frequenceBpsk = 0;
        private bool whichMode = true;//在作bpsk\boc图时，标志选择了bpsk还是boc
        private bool whichMode_2 = true;//在作误差分析时，标志选择了bpsk还是boc
        private int alpha = 0;//boc的参数α
        private int beta = 0;//bpsk的参数β

        //这7个是绑定到radioButton组的参数
        private bool timeDomain = false;
        private bool psd = true;
        private bool autocorrelation = false;

        private bool sLine = true;
        private bool delay = false;
        private bool cN = false;
        private bool multiPath = false;

        public double FrequenceBpsk
        {
            get
            {
                return frequenceBpsk;
            }
            set
            {
                frequenceBpsk = value;
                OnPropertyChanged("FrequenceBpsk");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public int Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
                OnPropertyChanged("Alpha");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public int Beta
        {
            get
            {
                return beta;
            }
            set
            {
                beta = value;
                OnPropertyChanged("Beta");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public bool WhichMode
        {
            get
            {
                return whichMode;
            }
            set
            {
                whichMode = value;
                OnPropertyChanged("WhichMode");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public bool WhichMode_2
        {
            get
            {
                return whichMode_2;
            }
            set
            {
                whichMode_2 = value;
                OnPropertyChanged("WhichMode_2");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public bool TimeDomain
        {
            get
            {
                return timeDomain;
            }
            set
            {
                timeDomain = value;
                OnPropertyChanged("TimeDomain");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public bool Psd
        {
            get
            {
                return psd;
            }
            set
            {
                psd = value;
                OnPropertyChanged("Psd");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public bool Autocorrelation
        {
            get
            {
                return autocorrelation;
            }
            set
            {
                autocorrelation = value;
                OnPropertyChanged("Autocorrelation");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public bool SLine
        {
            get
            {
                return sLine;
            }
            set
            {
                sLine = value;
                OnPropertyChanged("SLine");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public bool Delay
        {
            get
            {
                return delay;
            }
            set
            {
                delay = value;
                OnPropertyChanged("Delay");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public bool CN
        {
            get
            {
                return cN;
            }
            set
            {
                cN = value;
                OnPropertyChanged("CN");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

        public bool MultiPath
        {
            get
            {
                return multiPath;
            }
            set
            {
                multiPath = value;
                OnPropertyChanged("MultiPath");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }
        #endregion
        #region 方法
        public event PropertyChangedEventHandler PropertyChanged;//创建属性更改事件
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));//订阅方法
        }
#endregion
    }
}
