using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SoftwareDesign_2017
{
    class GraphicContext:INotifyPropertyChanged
    {
        private double labelX_0 = 0;
        private double labelX_1 = 0;
        private double labelX_2 = 0;
        private double labelX_3 = 0;
        private double labelX_4 = 0;
        private double labelX_5 = 0;
        private double labelX_6 = 0;

        private double labelY_0 = 0;
        private double labelY_1 = 0;
        private double labelY_2 = 0;
        private double labelY_3 = 0;
        private double labelY_4 = 0;
        private double labelY_5 = 0;
        private double labelY_6 = 0;
        private double labelY_7 = 0;
        private double labelY_8 = 0;

        private string xLabel;
        private string yLabel;        

        private string position;


        public double LabelX_0
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
        public double LabelX_1
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
        public double LabelX_2
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
        public double LabelX_3
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
        public double LabelX_4
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
        public double LabelX_5
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
        public double LabelX_6
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

        public double LabelY_0
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

        public double LabelY_1
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

        public double LabelY_2
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

        public double LabelY_3
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

        public double LabelY_4
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

        public double LabelY_5
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

        public double LabelY_6
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

        public double LabelY_7
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

        public double LabelY_8
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
