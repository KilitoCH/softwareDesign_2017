using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// 用于和graphic类进行数据绑定的类
    /// </summary>
    class GraphicContext:INotifyPropertyChanged
    {
        //以下都是坐标系的内容
        private string labelX_0;
        private string labelX_1;
        private string labelX_2;
        private string labelX_3;
        private string labelX_4;
        private string labelX_5;
        private string labelX_6;

        private string labelY_0;
        private string labelY_1;
        private string labelY_2;
        private string labelY_3;
        private string labelY_4;
        private string labelY_5;
        private string labelY_6;
        private string labelY_7;
        private string labelY_8;

        private string xLabel;
        private string yLabel;        

        //鼠标相对于drawingCanvas控件的位置
        private string position;


        public string LabelX_0
        {
            get
            {
                return labelX_0;
            }
            set
            {
                labelX_0 = value;
                OnPropertyChanged("LabelX_0");
            }
        }
        public string LabelX_1
        {
            get
            {
                return labelX_1;
            }
            set
            {
                labelX_1 = value;
                OnPropertyChanged("LabelX_1");
            }
        }
        public string LabelX_2
        {
            get
            {
                return labelX_2;
            }
            set
            {
                labelX_2 = value;
                OnPropertyChanged("LabelX_2");
            }
        }
        public string LabelX_3
        {
            get
            {
                return labelX_3;
            }
            set
            {
                labelX_3 = value;
                OnPropertyChanged("LabelX_3");
            }
        }
        public string LabelX_4
        {
            get
            {
                return labelX_4;
            }
            set
            {
                labelX_4 = value;
                OnPropertyChanged("LabelX_4");
            }
        }
        public string LabelX_5
        {
            get
            {
                return labelX_5;
            }
            set
            {
                labelX_5 = value;
                OnPropertyChanged("LabelX_5");
            }
        }
        public string LabelX_6
        {
            get
            {
                return labelX_6;
            }
            set
            {
                labelX_6 = value;
                OnPropertyChanged("LabelX_6");
            }
        }

        public string LabelY_0
        {
            get
            {
                return labelY_0;
            }
            set
            {
                labelY_0 = value;
                OnPropertyChanged("LabelY_0");
            }
        }

        public string LabelY_1
        {
            get
            {
                return labelY_1;
            }
            set
            {
                labelY_1 = value;
                OnPropertyChanged("LabelY_1");
            }
        }

        public string LabelY_2
        {
            get
            {
                return labelY_2;
            }
            set
            {
                labelY_2 = value;
                OnPropertyChanged("LabelY_2");
            }
        }

        public string LabelY_3
        {
            get
            {
                return labelY_3;
            }
            set
            {
                labelY_3 = value;
                OnPropertyChanged("LabelY_3");
            }
        }

        public string LabelY_4
        {
            get
            {
                return labelY_4;
            }
            set
            {
                labelY_4 = value;
                OnPropertyChanged("LabelY_4");
            }
        }

        public string LabelY_5
        {
            get
            {
                return labelY_5;
            }
            set
            {
                labelY_5 = value;
                OnPropertyChanged("LabelY_5");
            }
        }

        public string LabelY_6
        {
            get
            {
                return labelY_6;
            }
            set
            {
                labelY_6 = value;
                OnPropertyChanged("LabelY_6");
            }
        }

        public string LabelY_7
        {
            get
            {
                return labelY_7;
            }
            set
            {
                labelY_7 = value;
                OnPropertyChanged("LabelY_7");
            }
        }

        public string LabelY_8
        {
            get
            {
                return labelY_8;
            }
            set
            {
                labelY_8 = value;
                OnPropertyChanged("LabelY_8");
            }
        }

        public string XLabel
        {
            get
            {
                return xLabel;
            }
            set
            {
                xLabel = value;
                OnPropertyChanged("xLabel");
            }
        }

        public string YLabel
        {
            get
            {
                return yLabel;
            }
            set
            {
                yLabel = value;
                OnPropertyChanged("YLabel");
            }
        }

        public string Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                OnPropertyChanged("Position");
            }
        }        

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
