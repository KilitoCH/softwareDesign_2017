using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// 用于获取码跟踪精度误差的类
    /// </summary>
    class Error
    {
        private int stepLength = 10000;

        public List<Point> Varone(List<Point> points, int delta)
        {
            List<Point> spoints = new List<Point>();
            int a; //表示自变量∈

            for (a = -50; a <= 50; a++)
            {
                spoints.Add(new Point(a, points[points.Count / 2 - delta / 2 + a].Y * points[points.Count / 2 - delta / 2 + a].Y - points[points.Count / 2 + delta / 2 + a].Y * points[points.Count / 2 + delta / 2 + a].Y));
            }
            return spoints;
        }

        /// <summary>
        /// 误差的方差随cNDb（信噪比）变化的点序列
        /// </summary>
        /// <param name="List<Point> points">原G(f)采样点集合</param>
        /// <returns=vapoints>码跟踪误差随超前滞后本地码之间的时间间隔而变化的集合</returns>
        public List<Point> SigmaCNDbSequenceGenerate(List<Point> points, string name)
        {
            double _GF;
            double bL = 1;
            double T = 0.02;//相关积分时间
            double cN;//C/No  
            double sigma;///NELP码跟踪误差方差
            double delta;
            switch (name)
            {
                case "BPSK":
                    delta = 48.876 * Math.Pow(10, -9);
                    break;
                case "BOC(5,2)":
                    delta = 80 * Math.Pow(10, -9);
                    break;
                case "BOC(8,4)":
                    delta = 50 * Math.Pow(10, -9);
                    break;
                case "BOC(10,5)":
                    delta = 40 * Math.Pow(10, -9);
                    break;
                default:
                    delta = 80 * Math.Pow(10, -9);
                    break;
            }
            List<Point> vapoints = new List<Point>();
            Point vapoint = new Point();
            for (int cNDb = 20; cNDb <= 40; cNDb++)
            {
                cN = 10 * Math.Log10(cNDb);
                double tempVal1 = 0, tempVal2 = 0, tempVal3 = 0;//这三个变量用于存储三个积分
                double part1, part2, part3 = 0, part4;//这四个临时变量用于存储计算式的四个部分
                double deltaI = delta + 0.000000000001;//避免出现零除而对自变量加上少量偏移
                foreach (var point in points)
                {
                    if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal1 += _GF * Math.Pow(Math.Sin(Math.PI * point.X * deltaI), 2) * stepLength;
                    }
                }
                part1 = bL * (1 - 0.25 * bL * T) * tempVal1;
                ///左上角第一部分
                foreach (var point in points)
                {
                    if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal2 += point.X * _GF * Math.Sin(Math.PI * point.X * deltaI) * stepLength;
                    }
                }
                part2 = cN * Math.Pow((2 * Math.PI * tempVal2), 2);
                ///左下角第二部分
                foreach (var point in points)
                {
                    if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        part3 += _GF * Math.Pow(Math.Cos(Math.PI * point.X * deltaI), 2) * stepLength;
                    }
                }
                ///右上角第三部分
                foreach (var point in points)
                {
                    if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal3 += _GF * Math.Cos(Math.PI * point.X * deltaI) * stepLength;
                    }
                }
                part4 = T * cN * tempVal3 * tempVal3;
                ///右下角第四部分
                if
                    (part4 == 0 || part2 == 0) sigma = 0;//不确定分母是否取零
                else
                    sigma = part1 * (1 + part3 / part4) / part2;
                ///四部分总公式求得方差
                //vapoint.X = delta * 1000000000; //x
                vapoint.X = cNDb;
                vapoint.Y = Math.Sqrt(sigma) * 3 * Math.Pow(10, 8);//y
                vapoints.Add(vapoint);
            }
            return vapoints;
        }

        /// <summary>
        /// 误差的方差随delta（时移）变化的点序列
        /// </summary>
        /// <param name="List<Point> points">原G(f)采样点集合</param>
        /// <returns=vapoints>码跟踪误差随超前滞后本地码之间的时间间隔而变化的集合</returns>
        public List<Point> SigmaDeltaSequenceGenerate(List<Point> points)
        {
            double _GF;
            double bL = 1;
            double T = 0.02;//相关积分时间
            double cN = Math.Pow(10, 3);//C/No  
            double sigma;///NELP码跟踪误差方差
            double delta;
            List<Point> vapoints = new List<Point>();
            Point vapoint = new Point();
            for (delta = 0.00000002; delta <= 0.00000006001; delta = delta + 0.000000001) //自变量时间差△
            {
                double tempVal1 = 0, tempVal2 = 0, tempVal3 = 0;//这三个变量用于存储三个积分
                double part1, part2, part3 = 0, part4;//这四个临时变量用于存储计算式的四个部分
                double deltaI = delta + 0.000000000001;//避免出现零除而对自变量加上少量偏移
                foreach (var point in points)
                {
                    if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal1 += _GF * Math.Pow(Math.Sin(Math.PI * point.X * deltaI), 2) * stepLength;
                    }
                }
                part1 = bL * (1 - 0.25 * bL * T) * tempVal1;
                ///左上角第一部分
                foreach (var point in points)
                {
                    if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal2 += point.X * _GF * Math.Sin(Math.PI * point.X * deltaI) * stepLength;
                    }
                }
                part2 = cN * Math.Pow((2 * Math.PI * tempVal2), 2);
                ///左下角第二部分
                foreach (var point in points)
                {
                    if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        part3 += _GF * Math.Pow(Math.Cos(Math.PI * point.X * deltaI), 2) * stepLength;
                    }
                }
                ///右上角第三部分
                foreach (var point in points)
                {
                    if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal3 += _GF * Math.Cos(Math.PI * point.X * deltaI) * stepLength;
                    }
                }
                part4 = T * cN * tempVal3 * tempVal3;
                ///右下角第四部分
                if
                    (part4 == 0 || part2 == 0) sigma = 0;//不确定分母是否取零
                else
                    sigma = part1 * (1 + part3 / part4) / part2;
                ///四部分总公式求得方差
                vapoint.X = delta * 1000000000; //x
                vapoint.Y = Math.Sqrt(sigma) * 3 * Math.Pow(10, 8);//y
                vapoints.Add(vapoint);
            }
            return vapoints;
        }

        /// <summary>
        /// 计算镜像多径效应引起的偏移误差
        /// </summary>
        /// <param name="psdPoints">要计算的信号的功率谱密度</param>
        /// <param name="points_1">当多径反射信号相对于直达信号相位为2π偶数倍时得到的结果</param>
        /// <param name="points_2">当多径反射信号相对于直达信号相位为2π奇数倍时得到的结果</param>
        /// <param name="name">信号的类型</param>
        /// <returns></returns>
        public bool MultiPathSequenceGenerate(List<Point> psdPoints,out List<Point> points_1,out List<Point> points_2,string name)
        {
            try
            {
                double delta;
                int d;//多径时延
                points_1 = new List<Point>();
                points_2 = new List<Point>();
                switch (name)
                {
                    case "BPSK":
                        delta = 48.876 * Math.Pow(10, -9);
                        break;
                    case "BOC(5,2)":
                        delta = 80 * Math.Pow(10, -9);
                        break;
                    case "BOC(8,4)":
                        delta = 50 * Math.Pow(10, -9);
                        break;
                    case "BOC(10,5)":
                        delta = 40 * Math.Pow(10, -9);
                        break;
                    default:
                        delta = 80 * Math.Pow(10, -9);
                        break;
                }
                for (d = 0; d <= 250; d += 10)
                {
                    double tempVal1 = 0, tempVal2_1 = 0, tempVal2_2 = 0;
                    double part1, part2_1, part2_2;
                    double r = Math.Pow(10, -0.6);//镜像反射的幅度
                    foreach (var point in psdPoints)
                    {
                        if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                        {
                            tempVal1 += point.Y * Math.Sin(Math.PI * point.X * delta) * Math.Sin(2 * Math.PI * point.X * d / 1000000000) * stepLength;
                        }
                    }
                    part1 = r * tempVal1;
                    foreach (var point in psdPoints)
                    {
                        if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                        {
                            tempVal2_1 += point.X * point.Y * Math.Sin(Math.PI * point.X * delta) * (1 + r * Math.Cos(2 * Math.PI * point.X * d / 1000000000)) * stepLength;
                        }
                    }
                    part2_1 = 2 * Math.PI * tempVal2_1;
                    foreach (var point in psdPoints)
                    {
                        if (point.X >= -12000000 && point.X <= 12000000) ///带宽β选择24MHz
                        {
                            tempVal2_2 += point.X * point.Y * Math.Sin(Math.PI * point.X * delta) * (1 - r * Math.Cos(2 * Math.PI * point.X * d / 1000000000)) * stepLength;
                        }
                    }
                    part2_2 = 2 * Math.PI * tempVal2_2;
                    points_1.Add(new Point(d, part1 / part2_1));
                    points_2.Add(new Point(d, -part1 / part2_2));
                }
                return true;
            }
            catch
            {
                points_1 = null;
                points_2 = null;
                return false;
            }
        }
    }
}
