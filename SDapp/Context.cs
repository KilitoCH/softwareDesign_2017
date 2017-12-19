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
        public List<Point> pointsForBpsk = new List<Point>();//选项卡1的图片的点阵
        public List<Point> pointsForBoc = new List<Point>();//选项卡2的图片的点阵

        public Visual visualForBpsk;//选项卡1的图片的Visual格式字段
        public Visual visualForBoc;//选项卡2的图片的Visual格式字段

        private int frequenceUnit = 2;
        private double frequenceBpsk = 0;
        private int alpha = 0;
        private int beta = 0;

        public int FrequenceUnit
        {
            get
            {
                return frequenceUnit;
            }
            set
            {
                frequenceUnit = value;
                OnPropertyChanged("FrequenceUnit");//订阅这个事件可以向WPF发出通知我这个属性的值有更改
            }
        }

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

        #endregion
        #region 方法
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
#endregion
    }
}
