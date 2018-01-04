using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;

namespace SoftwareDesign_2017
{
    class Param
    {
        #region 字段
        private int boundWidth;//发射带宽
        private int foreBW;//接收机的前端带宽
        private int stepLength;//发送带宽的量化步进
        private int acCount;//自相关序列的量化步进
        private int val;//谱密度序列中取前端带宽的最左一个点

        private string name;///波形类型
        private double frequenceDelta;///频谱主瓣距频带中心的频偏
        private double psdMax = 0;///主瓣最大功谱密度
        private double lambda;///带限功率λ
        private double nintyPercentBW;///90%功率的带宽
        private double waste;///带外的损失
        private double betaSq;///带限信号的均方根带宽
        private double betaRect;///等效矩形带宽
        private double klsWithSelf;///与自身的频谱隔离系数
        private double klsWithBpsk;///与 1．023M H zBPSK 的频谱隔离系数
        private double klsWithBoc;///与BOC (10，5)的频谱隔离系数
        private int delay;///自相关函数主峰与第一副峰间的时延
        private double peakCompare;///自相关函数第一副峰与主峰幅度平方之比
        #endregion
        #region 构造函数
        /// <summary>
        ///参数计算
        /// </summary>
        /// <param name="name">BPSK或BOC信号的类型</param>
        /// <param name="psdSequence">BPSK或BOC信号的频率谱密度采样点集合</param>
        /// <param name="autoCorrelationSequence">BPSK或BOC信号的自相关函数的点集合</param>
        /// <returns></returns>
        public Param(string name, List<Point> psdSequence, List<Point> autoCorrelationSequence)
        {
            //从文件中读取初始化参数
            FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Open, FileAccess.Read);//打开参数存储文件并写入文件流
            StreamReader streamReader = new StreamReader(fileStream);//创建文件流写入类
            string[] str = streamReader.ReadLine().Split(',');//从文件中读取字符串，并以','为间隔提取出参数
            boundWidth = Convert.ToInt32(str[0]);//如下四行为将参数存储到本类的私有字段中
            foreBW = Convert.ToInt32(str[1]);
            stepLength = boundWidth / Convert.ToInt32(str[2]);
            acCount = Convert.ToInt32(str[3]);
            val = (boundWidth - foreBW) / (2 * stepLength);

            BPSK_Sequence_Generate bpsk = new BPSK_Sequence_Generate(1.023);///实例化1.023MHzBPSK 
            BOC_Sequence_Generate boc = new BOC_Sequence_Generate(10, 5);///实例化BOC（10,5）
            frequenceDelta = GetFrequenceDelta(psdSequence);///求得频谱主瓣距频带中心的频偏
            lambda = LambdaGet(psdSequence);///求得带限功率λ
            nintyPercentBW = GetNintypercentBW(psdSequence);///求得90%功率的带宽
            waste = GetWaste(psdSequence);///求得带外的损失
            betaSq = RMS(psdSequence);///求得带限信号的均方根带宽
            betaRect = Bandwidth(psdSequence);///求得等效矩形带宽
            klsWithSelf = KLS(psdSequence, psdSequence);///求得与自身的频谱隔离系数
            klsWithBpsk = KLS(psdSequence, bpsk.GetPsdSequenceReal);///求得与 1．023M H zBPSK 的频谱隔离系数
            klsWithBoc = KLS(psdSequence, boc.GetPsdSequenceReal);///求得与BOC (10，5)的频谱隔离系数
            delay = GetDelay(autoCorrelationSequence);///求得自相关函数主峰与第一副峰间的时延、自相关函数第一副峰与主峰幅度平方之比
            this.name = name;///得到选择的波形类型
        }
        #endregion
        #region 方法
        /// <summary>
        /// 参数计算BPSK或BOC的带限功率λ(lambda)
        /// </summary>
        /// <param name="points">BPSK或BOC信号的频率谱密度采样点集合</param>
        /// <returns=>BPSK或BOC的带限功率lamda值</returns>
        private double LambdaGet(List<Point> points)
        {
            double lambda = 0;///设lambda初始值为0
            foreach (var point in points)
            {
                if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2))  ///带宽β选择24MHz
                {
                    lambda += point.Y * stepLength;///用小分段累加近似代替积分
                }
            }
            Console.WriteLine(lambda);
            return lambda;
        }

        /// <summary>
        /// 计算BPSK或BOC的带限信号的均方根带宽
        /// </summary>
        /// <param name="points">BPSK或BOC信号的功率谱密度采样点集合</param>
        /// <param name="lambda">信号的带限后的信号剩余功率部分λ</param>
        /// <returns="w2">带限信号的均方根带宽</returns>r
        private Double RMS(List<Point> points)
        {
            double G = 0;//归一化后的G杠
            double betaSq = 0;//betaSq表示带宽的平方
            foreach (var point in points)
            {
                if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2))  ///带宽β选择24MHz
                {
                    G = point.Y / lambda;///归一化
                    betaSq = betaSq + point.X * point.X * G * stepLength;///用小分段累加近似代替积分
                }
            }
            return Math.Sqrt(betaSq) / 1000000;///单位为Mhz
        }

        /// <summary>
        /// 参数计算有效矩形带宽w（βrect=λ/Gs(fmax)）
        /// </summary>
        /// <param name="points">BPSK或BOC频率谱密度采样点集合</param>
        /// <param name="lambda">BPSK或BOC信号的带限后的信号剩余功率部分λ</param>
        /// <returns="betaRect">BPSK或BOC信号的有效矩形带宽βrect</returns>
        private double Bandwidth(List<Point> points)
        {
            double Gmax = 0;///设一个小初始值
            double betaRect;
            foreach (var point in points)
            {
                if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2))  ///带宽β选择24MHz
                {
                    if (point.Y > Gmax)
                    {
                        Gmax = point.Y;
                    }                                       ///遍历求得最大值
                }
            }
            betaRect = lambda / Gmax;
            return betaRect / 1000000;///单位为Mhz
        }

        /// <summary>
        /// BPSK或BOC信号的频谱隔离系数Kls的参数计算Kls=∫(-B/2,B/2) Gl'(f)・Gs(f)df
        /// </summary>
        /// <param name="points1">信号的功率谱密度采样点集合</param>
        /// <param name="points2">干扰的功率谱密度采样点集合</param>
        /// <returns="Kls">频谱隔离系数Kls</returns>
        private double KLS(List<Point> points1, List<Point> points2)
        {
            double kls = 0;
            var jam_sum = Jam_sum(points2);///调用干扰信号的处理
            for (int i = val; i <= points1.Count - val; i++)     ///带宽βr选择24MHz,每个点间隔为1000
            {
                kls = kls + points1[i].Y * points2[i].Y * stepLength / jam_sum;///用小分段累加近似代替积分
            }
            kls = 10 * Math.Log10(kls);  ///取对数使之对应单位为db
            return kls;
        }

        /// <summary>
        /// 对干扰信号进行归一化预处理
        /// </summary>
        /// <param name="points2">干扰的功率谱密度采样点集合</param>
        /// <returns="Pl">噪声在有限带宽中的总功率Pl</returns>
        private double Jam_sum(List<Point> points2)
        {
            double Pl = 0;
            foreach (var point2 in points2)
            {
                if (point2.X >= -(foreBW / 2) && point2.X <= (foreBW / 2))   ///带宽βr选择24MHz(可变)
                {
                    Pl += point2.Y * stepLength;///用小分段累加近似代替积分
                }
            }
            return Pl;///噪声在有限带宽中的总功率Pl
        }
        /// <summary>
        /// 求得频谱主瓣距频带中心的频偏 (MHz)
        /// </summary>
        /// <param name="points">信号的功率谱密度采样点集合</param>
        /// <returns="Math.Abs(delta)/1000000">取模后的频偏，再进行单位换算</returns>
        private double GetFrequenceDelta(List<Point> points)
        {
            double delta = 0;///设定一个小初始值
            foreach (var point in points)
            {
                if (point.Y > psdMax)
                {
                    delta = point.X;
                    psdMax = point.Y;///遍历求得功率谱密度最大值
                }
            }
            return Math.Abs(delta) / 1000000;
        }
        /// <summary>
        /// 求得90%功率的带宽 (MHz)
        /// </summary>
        /// <param name="points">信号的功率谱密度采样点集合</param>
        /// <returns="2 * points[i].X / 1000000">90%功率的带宽</returns>
        private double GetNintypercentBW(List<Point> points)
        {
            double power = 0;//功率临时变量
            int i;
            for (i = points.Count / 2; i < points.Count; i++)///从中轴往右边找到一半带宽
            {
                power += points[i].Y * 2 * stepLength;
                if (power >= (lambda * 0.9))
                    break;
            }
            return 2 * points[i].X / 1000000;///一半带宽乘2，在进行单位换算
        }

        /// <summary>
        /// 求得带外的损失 (dB)
        /// </summary>
        /// <param name="points">信号的功率谱密度采样点集合</param>
        /// <returns="10 * Math.Log10(power/lambda">带外的损失 (dB)</returns>
        private double GetWaste(List<Point> points)
        {
            double power = 0;//功率临时变量
            foreach (var point in points)
            {
                power += point.Y * stepLength;///累和代替积分
            }
            return 10 * Math.Log10(power / lambda);///归一化后进行单位换算
        }
        /// <summary>
        /// 求得自相关函数主峰与第一副峰间的时延、自相关函数第一副峰与主峰幅度平方之比
        /// </summary>
        /// <param name="acPoints">信号的功率谱密度采样点集合</param>
        /// <returns="delay * 1000000000">单位为ns的时延</returns>
        private int GetDelay(List<Point> acPoints)
        {
            int i;
            double ySecondMax = 0;///初始化第一副峰的最大值
            double delay = 0;///初始化时延
            if (acPoints != null)
            {
                for (i = acPoints.Count / 2; i < acPoints.Count; i++)///由于对称，只用研究一边。此处找到主峰的低处
                {
                    if (acPoints[i].Y < 0.05)
                        break;
                }
                for (; i < acPoints.Count; i++)///i从主峰的低处往右边找，遍历找到第一个最大的点
                {
                    if (acPoints[i].Y >= ySecondMax)//比较遍历
                    {
                        ySecondMax = acPoints[i].Y;
                        delay = acPoints[i].X;
                    }
                }
            }
            peakCompare = Math.Pow(ySecondMax, 2);///自相关函数第一副峰与主峰幅度平方之比
            return (int)(delay * 1000000000);///返回时延
        }
        #endregion
        #region 属性
        public double Lambda
        {
            get { return lambda; }
        }

        public double BetaRect
        {
            get { return betaRect; }
        }


        public double BetaSq
        {
            get { return betaSq; }
        }

        public double KlsWithSelf
        {
            get { return klsWithSelf; }
        }

        public double KlsWithBpsk
        {
            get { return klsWithBpsk; }
        }

        public double KlsWithBoc
        {
            get { return klsWithBoc; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public double FrequenceDelta
        {
            get { return frequenceDelta; }
        }

        public double PsdMax
        {
            get { return 10 * Math.Log10(psdMax); }
        }

        public double NintyPercentBW
        {
            get { return nintyPercentBW; }
        }

        public double Waste
        {
            get { return waste; }
        }

        public int Delay
        {
            get { return delay; }
        }

        public double PeakCompare
        {
            get { return peakCompare; }
        }
        #endregion
    }
}

