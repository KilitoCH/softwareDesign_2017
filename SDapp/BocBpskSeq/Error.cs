using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;

namespace SoftwareDesign_2017
{
    /// <summary>
    /// 用于获取码跟踪精度误差的类
    /// </summary>
    class Error
    {
        private int boundWidth;//发射带宽
        private int foreBW;//接收机的前端带宽
        private int stepLength;//发送带宽的量化步进
        private int AcPointCount;//自相关序列的量化步进

        public Error()
        {
            //从文件中预读取参数
            FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Open, FileAccess.Read);
            StreamReader streamReader = new StreamReader(fileStream);
            string[] str = streamReader.ReadLine().Split(',');
            boundWidth = Convert.ToInt32(str[0]);
            foreBW = Convert.ToInt32(str[1]);
            stepLength = boundWidth / Convert.ToInt32(str[2]);
            AcPointCount = Convert.ToInt32(str[3]);
        }

        /// <summary>
        /// S曲线，即表示的是鉴相器跟踪误差与其所产生的校正量之间的函数
        /// </summary>
        /// <param name="points">原G(f)采样点集合</param>
        /// <param name="delta">超前一滞后本地码的间隔△</param>
        /// <returns=spoints>S曲线的函数取样点集合</returns>
        public List<Point> Varone(List<Point> points, int delta)
        {
            List<Point> spoints = new List<Point>();
            int a; //表示自变量∈

            for (a = -50; a <= 50; a++)///横坐标∈只需要从-50到+50ns之间变化
            {
                ///直接利用之前所求的归一化的自相关序列的点集合，自相关序列的中点points[points.Count/2] 为R(0)，points[points.Count / 2 - delta / 2 + a]即为R(∈一△／2)
                spoints.Add(new Point(a, Math.Pow(points[points.Count / 2 - delta / 2 + a].Y, 2) - Math.Pow(points[points.Count / 2 + delta / 2 + a].Y, 2)));
            }
            return spoints;///S曲线的函数取样点集合
        }

        /// <summary>
        /// 误差的方差随cNDb（单位为db的信噪比）变化的点序列
        /// </summary>
        /// <param name="List<Point> points">原G(f)采样点集合</param>
        /// <param name="name">选择波形种类名称</param>
        /// <returns=vapoints>码跟踪误差随cNDb而变化的点集合</returns>
        public List<Point> SigmaCNDbSequenceGenerate(List<Point> points, string name)
        {
            double _GF;///功率谱密度
            double bL = 1;///Bl为1Hz
            double T = 0.02;///相关积分时间,20ms
            double cN;///C/No(信噪比)
            double sigma;///NELP码跟踪误差方差
            double delta;///超前一滞后本地码的间隔△
            switch (name)///选择波形类型
            {
                ///1.023MHz的BPSK 为0.05基码宽度,10.23Mhz为0.5基码
                case "BPSK":
                    delta = 48.876 * Math.Pow(10, -9);
                    break;
                ///BOC (5，2)△为80ns
                case "BOC(5,2)":
                    delta = 80 * Math.Pow(10, -9);
                    break;
                ///BOC (8，4)△为50ns
                case "BOC(8,4)":
                    delta = 50 * Math.Pow(10, -9);
                    break;
                ///BOC(10，5)来说,△取40ns
                case "BOC(10,5)":
                    delta = 40 * Math.Pow(10, -9);
                    break;
                ///default选择80ns
                default:
                    delta = 80 * Math.Pow(10, -9);
                    break;
            }
            List<Point> vapoints = new List<Point>();
            Point vapoint = new Point();
            for (int cNDb = 20; cNDb <= 40; cNDb++)///横坐标选择从20db到40db
            {
                cN = Math.Pow(0.1 * cNDb, 10);///cN和cNDb之间的单位转化
                double tempVal1 = 0, tempVal2 = 0, tempVal3 = 0;//这三个变量用于存储三个积分
                double part1, part2, part3 = 0, part4;//这四个临时变量用于存储计算式的四个部分
                double deltaI = delta + 0.000000000001;//避免出现零除而对自变量加上少量偏移
                ///左上角第一部分
                foreach (var point in points)///依次从功率谱密度的点集中取点
                {
                    if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal1 += _GF * Math.Pow(Math.Sin(Math.PI * point.X * deltaI), 2) * stepLength;
                    }
                }
                part1 = bL * (1 - 0.25 * bL * T) * tempVal1;
                ///左下角第二部分
                foreach (var point in points)
                {
                    if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal2 += point.X * _GF * Math.Sin(Math.PI * point.X * deltaI) * stepLength;
                    }
                }
                part2 = cN * Math.Pow((2 * Math.PI * tempVal2), 2);
                ///右上角第三部分
                foreach (var point in points)
                {
                    if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        part3 += _GF * Math.Pow(Math.Cos(Math.PI * point.X * deltaI), 2) * stepLength;
                    }
                }
                ///右下角第四部分
                foreach (var point in points)
                {
                    if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal3 += _GF * Math.Cos(Math.PI * point.X * deltaI) * stepLength;
                    }
                }
                part4 = T * cN * tempVal3 * tempVal3;
                if
                    (part4 == 0 || part2 == 0) sigma = 0;//不确定分母是否取零从而输出无结果
                else
                    sigma = part1 * (1 + part3 / part4) / part2;///四部分总公式求得方差的平方
                vapoint.X = cNDb;
                vapoint.Y = Math.Sqrt(sigma) * 3 * Math.Pow(10, 8);//y，公式的基础上乘以光速，单位才变成m
                vapoints.Add(vapoint);
            }
            return vapoints;///码跟踪误差随cNDb而变化的点集合
        }

        /// <summary>
        /// 误差的方差随delta（时移）变化的点序列
        /// </summary>
        /// <param name="List<Point> points">原G(f)采样点集合</param>
        /// <returns=vapoints>码跟踪误差随超前滞后本地码之间的时间间隔△而变化的集合</returns>
        public List<Point> SigmaDeltaSequenceGenerate(List<Point> points)
        {
            double _GF;
            double bL = 1;
            double T = 0.02;//相关积分时间
            double cN = Math.Pow(10, 3);//C/No 取30db/hz,再进行单位换算
            double sigma;///NELP码跟踪误差方差
            double delta;///自变量超前滞后本地码之间的时间间隔△
            List<Point> vapoints = new List<Point>();
            Point vapoint = new Point();
            for (delta = 0.00000002; delta <= 0.00000006001; delta = delta + 0.000000001) //自变量时间差△，从20ns到60ns变化
            {
                double tempVal1 = 0, tempVal2 = 0, tempVal3 = 0;//这三个变量用于存储三个积分
                double part1, part2, part3 = 0, part4;//这四个临时变量用于存储计算式的四个部分
                double deltaI = delta + 0.000000000001;//避免出现零除而对自变量加上少量偏移
                ///左上角第一部分
                foreach (var point in points)
                {
                    if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal1 += _GF * Math.Pow(Math.Sin(Math.PI * point.X * deltaI), 2) * stepLength;
                    }
                }
                part1 = bL * (1 - 0.25 * bL * T) * tempVal1;
                ///左下角第二部分
                foreach (var point in points)
                {
                    if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal2 += point.X * _GF * Math.Sin(Math.PI * point.X * deltaI) * stepLength;
                    }
                }
                part2 = cN * Math.Pow((2 * Math.PI * tempVal2), 2);
                ////右上角第三部分
                foreach (var point in points)
                {
                    if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        part3 += _GF * Math.Pow(Math.Cos(Math.PI * point.X * deltaI), 2) * stepLength;
                    }
                }
                ///右下角第四部分
                foreach (var point in points)
                {
                    if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                    {
                        _GF = point.Y;//功率谱密度
                        tempVal3 += _GF * Math.Cos(Math.PI * point.X * deltaI) * stepLength;
                    }
                }
                part4 = T * cN * tempVal3 * tempVal3;
                if
                    (part4 == 0 || part2 == 0) sigma = 0;//不确定分母是否取零导致输出无结果
                else
                    sigma = part1 * (1 + part3 / part4) / part2;///四部分总公式求得方差的平方
                vapoint.X = delta * 1000000000; //X单位从s换算为ns
                vapoint.Y = Math.Sqrt(sigma) * 3 * Math.Pow(10, 8);//Y输出为方差，单位乘光速换算成m
                vapoints.Add(vapoint);
            }
            return vapoints;///码跟踪误差随超前滞后本地码之间的时间间隔△而变化的点集合
        }

        /// <summary>
        /// 计算镜像多径效应引起的偏移误差
        /// </summary>
        /// <param name="psdPoints">要计算的信号的功率谱密度</param>
        /// <param name="points_1">当多径反射信号相对于直达信号相位为2π偶数倍时得到的结果</param>
        /// <param name="points_2">当多径反射信号相对于直达信号相位为2π奇数倍时得到的结果</param>
        /// <param name="name">信号的类型</param>
        /// <returns></returns>
        public bool MultiPathSequenceGenerate(List<Point> psdPoints, out List<Point> points_1, out List<Point> points_2, string name)
        {
            try
            {
                double delta;
                int d;//多径时延
                points_1 = new List<Point>();///相位为2π偶数倍
                points_2 = new List<Point>();///相位为2π奇数倍
                switch (name)///选择类型
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
                for (d = 0; d <= 250; d += 10)///自变量选择在0到250ns之间变化
                {
                    double tempVal1 = 0, tempVal2_1 = 0, tempVal2_2 = 0;
                    double part1, part2_1, part2_2;
                    double r = Math.Pow(10, -0.6);//镜像反射的幅度
                    foreach (var point in psdPoints)///直接利用之前所求的功率谱密度序列
                    {
                        if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                        {
                            tempVal1 += point.Y * Math.Sin(Math.PI * point.X * delta) * Math.Sin(2 * Math.PI * point.X * d / 1000000000) * stepLength;///不带负号的分子的值
                        }
                    }
                    part1 = r * tempVal1;
                    foreach (var point in psdPoints)
                    {
                        if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                        {
                            tempVal2_1 += point.X * point.Y * Math.Sin(Math.PI * point.X * delta) * (1 + r * Math.Cos(2 * Math.PI * point.X * d / 1000000000)) * stepLength;///相位为2π偶数倍的公式
                        }
                    }
                    part2_1 = 2 * Math.PI * tempVal2_1;
                    foreach (var point in psdPoints)
                    {
                        if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2)) ///带宽β选择24MHz
                        {
                            tempVal2_2 += point.X * point.Y * Math.Sin(Math.PI * point.X * delta) * (1 - r * Math.Cos(2 * Math.PI * point.X * d / 1000000000)) * stepLength;///相位为2π奇数倍的公式
                        }
                    }
                    part2_2 = 2 * Math.PI * tempVal2_2;
                    points_1.Add(new Point(d, part1 / part2_1));///相位为2π偶数倍新添加点
                    points_2.Add(new Point(d, -part1 / part2_2));///相位为2π奇数倍新添加点
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
