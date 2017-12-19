using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace SoftwareDesign_2017
{
    class Variance
    {
        /// <summary>
        /// 方差的图形绘制
        /// </summary>
        /// <param name="frequence">正弦波的频率</param>
        /// <returns></returns>
        public List<Point> BPSK_variance(Double frequence)
        {
            double Ts = 1 / frequence; ;
            double betar = 24000000;
            double GF;
            double BL = 1;
            double T = 0.02;//相关积分时间
            double a = 0;
            double b;
            double c = 0;
            double d;
            double e = 0;
            double g = 0;
            double h;
            double p; ///NELP码跟踪误差方差
            var points = new List<Point>();
            Point point = new Point();
            for (double s = 0; s <= 0.0000001; s = s + 0.000000001) //自变量时间差△
            {
                for (double f = -(betar / 2) + 1; f < betar / 2; f = f + 10000)
                {
                    GF = Ts * Math.Sin(Math.PI * Ts * f) / (Math.PI * Ts *
                    f);//功率谱密度
                    a = a + GF * Math.Sin(Math.PI * f * s) * Math.Sin(Math.PI *
                    f * s) * 10000;
                }
                b = BL * (1 - 0.25 * BL * T) * a;
                ///左上角第一部分
                for (double f = -(betar / 2) + 1; f < betar / 2; f = f + 10000)
                {
                    GF = Ts * Math.Sin(Math.PI * Ts * f) / (Math.PI * Ts * f);
                    c = c + f * GF * Math.Sin(Math.PI * f * s) * 10000;
                }
                d = 30 * (2 * Math.PI * c) * (2 * Math.PI * c);
                ///左下角第二部分
                for (double f = -(betar / 2) + 1; f < betar / 2; f = f + 10000)
                {
                    GF = Ts * Math.Sin(Math.PI * Ts * f) / (Math.PI * Ts * f);
                    e = e + GF * Math.Cos(Math.PI * f * s) * Math.Cos(Math.PI *
                    f * s) * 10000;
                }
                ///右上角第三部分
                for (double f = -(betar / 2) + 1; f < betar / 2; f = f + 10000)
                {
                    GF = Ts * Math.Sin(Math.PI * Ts * f) / (Math.PI * Ts * f);
                    g = g + GF * Math.Cos(Math.PI * f * s) * 10000;
                }
                h = T * 30 * g * g;
                ///右下角第四部分
                if (h == 0 || d == 0) p = 0;
                else p = b * (1 + e / h) / d;
                ///四部分总公式求得方差
                point.X = (s) * 1000000000; //x
                point.Y = p * Math.Pow(10,18);//y
                                        //坐标转换
                XYTransfer(point, ref point);
                points.Add(point);
            }
            return points;
        }
        private void XYTransfer(Point xyPoint, ref Point point)
        {
            point.X = xyPoint.X + 10;
            point.Y = -xyPoint.Y + 250;
        }
    }
}