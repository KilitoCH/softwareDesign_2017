using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;

namespace SoftwareDesign_2017
{
    class BPSK_Sequence_Generate
    {
        #region 字段
        private int boundWidth;//发射带宽
        private int foreBW;//接收机的前端带宽
        private int stepLength;//发射带宽量化步长
        private int acCount;//自相关序列的点数

        private List<Point> bpskSequence;//BPSK时序列点集合
        private List<Point> psdSequenceReal;//原BPSK功率谱密度点集合
        private List<Point> psdSequenceRealDb;//单位转化为dB的BPSK功率谱密度点集合
        private List<Point> autocorrelationSequence;//BPSK自相关函数点集合

        #endregion
        #region 构造函数        
        /// <summary>
        /// 当输入参数不包含窗口的大小时根据输入的扩频码速率获取一个PSD序列
        /// </summary>
        /// <param name="frequence">伪随机码的速率</param>
        public BPSK_Sequence_Generate(double frequence)
        {
            FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Open, FileAccess.Read);//打开参数存储文件并写入文件流
            StreamReader streamReader = new StreamReader(fileStream);//创建文件流写入类
            string[] str = streamReader.ReadLine().Split(',');//从文件中读取字符串，并以','为间隔提取出参数
            boundWidth = Convert.ToInt32(str[0]);//如下四行为将参数存储到本类的私有字段中
            foreBW = Convert.ToInt32(str[1]);
            stepLength = boundWidth / Convert.ToInt32(str[2]);
            acCount = Convert.ToInt32(str[3]);
            //关闭文件流
            streamReader.Close();
            fileStream.Close();

            if (frequence != 0)///随机码的速率不能为零
            {
                frequence *= 1000000;///从输入MHz的单位转化为计算用的Hz的单位
                bpskSequence = BPSKSequenceGenerate(frequence);
                psdSequenceReal = BPSKPSDGenerate(frequence);
                psdSequenceRealDb = ChangeToDb(psdSequenceReal);
            }
            else
                throw (new Exception("请先输入参数"));
        }
        #endregion
        #region 方法
        /// <summary>
        /// 返回一个双极性NRZ调制信号
        /// </summary>
        /// <param name="frequence">伪随机码的速率</param>
        /// <returns="points">BPSK时域点集合</returns>
        private List<Point> BPSKSequenceGenerate(Double frequence)
        {
            List<Point> points = new List<Point>();
            Random random = new Random();///随机数的产生
            frequence /= 1000000;///单位转换
            for (int i = 0; i < 20; i++)
            {
                double ak = random.NextDouble();///随机返回一个大于或等于0.0且小于1.0的随机浮点数
                if (ak >= 0.5)///判断为大于等于0.5，电平置1
                {
                    ///取电平为1的两边界点
                    points.Add(new Point(i / frequence, 1));
                    points.Add(new Point((i + 1) / frequence, 1));
                }
                else///判断为小于0.5，电平置-1
                {
                    ///取电平为-1的两边界点
                    points.Add(new Point(i / frequence, -1));
                    points.Add(new Point((i + 1) / frequence, -1));
                }
            }
            return points;
        }

        /// <summary>
        /// 返回一个BPSK信号（未经调制的双极性NRZ调制信号）的功率谱密度
        /// </summary>
        /// <param name="frequence">频率</param>
        /// <returns="pointsReal">真实数值的BPSK的功率谱密度点序列</returns>
        private List<Point> BPSKPSDGenerate(Double frequence)
        {
            List<Point> pointsReal = new List<Point>();///原始的具有真实数值的PSD点序列
            Point point = new Point();///用于添加新点的临时变量
            for (int deltaI/*deltaI是为了避免零除点*/, i = -(boundWidth / 2); i <= (boundWidth / 2); i += stepLength)
            {
                deltaI = i + 1;///因为1对于兆的单位很小，所以可以看成极小偏移
                point.X = i;
                point.Y = Math.Pow(((Math.Sin(Math.PI * deltaI / frequence)) / (Math.PI * deltaI / frequence)), 2) / frequence;///公式
                pointsReal.Add(point);///把新点加到真实数值的PSD点序列
            }
            return pointsReal;///返回真实数值的BPSK的功率谱密度点序列
        }

        /// <summary>
        /// 从原始的psd数据得到直接变换后的db数据，并进行了将小于-100db的数据全部截取为-100db的处理
        /// </summary>
        /// <param name="pointsReal">原始的PSD序列</param>
        /// <returns="pointsRealDb"></returns>
        private List<Point> ChangeToDb(List<Point> pointsReal)
        {
            List<Point> pointsRealDb = new List<Point>();
            foreach (var point in pointsReal)
            {
                Point tempPoint = new Point(point.X, 10 * Math.Log10(point.Y));///定义一个中间变换点序列tempPoint，横坐标X和真实点序列相同，而Y进行w和dB之间的单位转换
                if (tempPoint.Y >= -100)///这里的判断语句是因为数值太小的dB总会小于-100，这时候会在画图框内出现一条条竖线，这是我们不希望看到的
                {
                    pointsRealDb.Add(tempPoint);///y轴数值大于-100的直接取真实值
                }
                else
                    pointsRealDb.Add(new Point(tempPoint.X, -100));///y轴数值小于-100的直接近似取-100
            }
            return pointsRealDb;///返回单位变为dB之后的点序列，方便画图
        }

        /// <summary>
        /// BPSK的归一化的自相关函数序列
        /// </summary>
        /// <param name="psdSequence">原始的PSD序列</param>
        /// <returns="rs">自相关函数序列</returns>
        private List<Point> AutocorrelationSequenceGenerate(List<Point> psdSequence)
        {
            double lambda = 0;///设λ的初始值为0
            int val = (boundWidth - foreBW) / (2 * stepLength);//谱密度序列中取前端带宽的最左一个点
            List<Point> rs = new List<Point>();//Rs(t)
            foreach (var point in psdSequence)///直接利用功率谱密度所求的点集合求λ,为自相关函数归一化做准备
            {
                if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2))  ///带宽β选择24MHz
                {
                    lambda += point.Y * stepLength;///用极小分段累加近似代替积分，stepLength为步长（步长取10000.一共取2401个点）
                }
            }
            for (double i = -0.000001; i <= 0.000001001; i += 0.000002 / acCount)///i为时间t，单位为s，点数可以设定。但是范围为-1ns到1ns。
            {
                double tempVal1 = 0;///实部
                double tempVal2 = 0;///虚部
                double tempVal3 = 0;///取模
                for (int j = val; j < psdSequence.Count - val; j++)///因为画图取的是30M，数值计算用24M，所以PSD序列有3000个点，舍去头尾，只要300到2700的数值，刚好有2400个点
                {
                    tempVal1 += psdSequence[j].Y * Math.Cos(2 * Math.PI * psdSequence[j].X * i) * stepLength;///Gs（f)*cos(2*PI*f*i)
                }
                for (int j = val; j < psdSequence.Count - val; j++)
                {
                    tempVal2 += psdSequence[j].Y * Math.Sin(2 * Math.PI * psdSequence[j].X * i) * stepLength;///Gs（f)*sin(2*PI*f*i)
                }
                tempVal3 = Math.Sqrt(Math.Pow(tempVal1, 2) + Math.Pow(tempVal2, 2)) / lambda;/// tempVal1的平方加上 tempVal2的平方开根号，再进行归一化
                rs.Add(new Point(i * 1000000, tempVal3));///加入rs的点集合中
            }
            return rs;///返回自相关函数的点集合
        }
        #endregion
        #region 属性
        /// <summary>
        /// 获取一个未经过坐标变换的BPSK信号的功率谱密度点序列
        /// </summary>
        public List<Point> GetPsdSequenceReal
        {
            get { return psdSequenceReal; }
        }

        /// <summary>
        /// 获取一个未经过坐标变换的BPSK信号的自相关函数点序列
        /// </summary>
        public List<Point> GetAutocorrelationSequence
        {
            get
            {
                if (autocorrelationSequence == null)///因为自相关序列计算相对麻烦，所以设置为只有真正被需要的时候才被调用
                    return autocorrelationSequence = AutocorrelationSequenceGenerate(psdSequenceReal);
                else
                    return autocorrelationSequence;
            }
        }

        /// <summary>
        /// 获取一个DB变换后，但未适应窗口的BPSK信号的功率谱密度点序列
        /// </summary>
        public List<Point> GetPsdSequenceRealDb
        {
            get { return psdSequenceRealDb; }
        }

        /// <summary>
        /// 获取一个双极性非归零码序列
        /// </summary>
        public List<Point> GetBpskSequence
        {
            get { return bpskSequence; }
        }
        #endregion
    }
}
