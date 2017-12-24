using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SoftwareDesign_2017
{
    class Param
    {
        private string name;
        private double frequenceDelta;
        private double lambda;
        private double betaSq;
        private double betaRect;
        private double klsWithSelf;
        private double klsWithBpsk;
        private double klsWithBoc;

        private int stepLength = 10000;


        public Param(string name,List<Point> signalPoints)
        {
            BPSK_Sequence_Generate bpsk = new BPSK_Sequence_Generate(1.023, 2);
            BOC_Sequence_Generate boc = new BOC_Sequence_Generate(10, 5);
            frequenceDelta = GetFrequenceDelta(signalPoints);
            lambda = LambdaGet(signalPoints);
            betaSq = RMS(signalPoints);
            betaRect = Bandwidth(signalPoints);
            klsWithSelf = KLS(signalPoints,signalPoints);
            klsWithBpsk = KLS(signalPoints, bpsk.GetPsdSequenceReal);
            klsWithBoc = KLS(signalPoints, boc.GetPsdSequenceReal);

            this.name = name;
        }

        /// <summary>
        /// 参数计算BPSK或BOC的带限功率λ(lambda)
        /// </summary>
        /// <param name="points">BPSK或BOC信号的频率谱密度采样点集合</param>
        /// <returns=>BPSK或BOC的带限功率lamda值</returns>
        private double LambdaGet(List<Point> points)
        {
                               
            double lambda = 0;
            foreach (var point in points) 
            {
                if(point.X >= -12000000 && point.X <= 12000000)  ///带宽β选择24MHz
                {
                    lambda += point.Y * stepLength;
                }
            }
            return lambda;
        }

        /// <summary>
        /// 计算BPSK或BOC的带限信号的均方根带宽
        /// </summary>
        /// <param name="points">BPSK或BOC信号的功率谱密度采样点集合</param>
        /// <param name="lambda">信号的带限后的信号剩余功率部分λ</param>
        /// <returns="w2">带限信号的均方根带宽</returns>
        private Double RMS(List<Point> points)
        {
            double G = 0;//表示G杠,归一化
            double betaSq = 0;//betaSq表示带宽的平方
            foreach (var point in points)
            {
                if (point.X >= -12000000 && point.X <= 12000000)  ///带宽β选择24MHz
                {
                    G = point.Y / lambda;
                    betaSq = betaSq + point.X * point.X * G * stepLength;
                }
            }
            return Math.Sqrt(betaSq) / 1000000;
        }

        /// <summary>
        /// 参数计算有效矩形带宽w（βrect=λ/Gs(fmax)）
        /// </summary>
        /// <param name="points">BPSK或BOC频率谱密度采样点集合</param>
        /// <param name="lambda">BPSK或BOC信号的带限后的信号剩余功率部分λ</param>
        /// <returns="betaRect">BPSK或BOC信号的有效矩形带宽βrect</returns>
        private double Bandwidth(List<Point> points)
        {
            double Gmax = 0;
            double betaRect = 0;
            foreach (var point in points)
            {
                if (point.X >= -12000000 && point.X <= 12000000)  ///带宽β选择24MHz
                {
                    if (point.Y > Gmax)
                    {
                        Gmax = point.Y;
                    }                                       ///遍历求得最大值
                }
            }
            betaRect = lambda / Gmax;
            return betaRect / 1000000;
        }

        /// <summary>
        /// BPSK或BOC信号的频谱隔离系数Kls的参数计算Kls=∫(-B/2,B/2) Gl'(f)・Gs(f)df
        /// </summary>
        /// <param name="points1">信号的功率谱密度采样点集合</param>
        /// <param name="points2">干扰的功率谱密度采样点集合</param>
        /// <param name="lambda">信号的带限后的信号剩余功率部分λ</param>
        /// <returns="Kls">频谱隔离系数Kls</returns>
        private double KLS(List<Point> points1, List<Point> points2)
        {
            double kls = 0;
            var jam_sum = Jam_sum(points2);
            for (int i = 300; i <= 2700; i ++)     ///带宽βr选择24MHz,每个点间隔为1000
            {
                kls = kls + points1[i].Y * points2[i].Y * stepLength / jam_sum;
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
                if (point2.X >= -12000000 && point2.X <= 12000000)   ///带宽βr选择24MHz(可变)
                {
                    Pl += point2.Y * stepLength;
                }
            }
            return Pl;
        }

        public double Lambda
        {
            get { return lambda;}
        }

        public double BetaRect
        {
            get { return betaRect;}
        }


        public double BetaSq
        {
            get { return betaSq;}
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

        private double GetFrequenceDelta(List<Point> points)
        {
            double yMax = 0;
            double delta = 0;
            foreach (var point in points)
            {
                if (point.Y >= yMax)
                    delta = point.X;
            }
            return Math.Abs(delta);
        }
    }
}
