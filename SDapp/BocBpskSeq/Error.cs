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
                vapoint.Y = sigma * 3 * Math.Pow(10, 8);//y
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
                vapoint.Y = sigma * 3 * Math.Pow(10, 8);//y
                vapoints.Add(vapoint);
            }
            return vapoints;
        }
    }
}
