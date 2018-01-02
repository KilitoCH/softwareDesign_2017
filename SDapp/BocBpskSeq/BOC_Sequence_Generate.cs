using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SoftwareDesign_2017
{
    class BOC_Sequence_Generate
    {
#region 字段
        private const int boundWidth = 30000000;//接收机的前端带宽
        private List<Point> bocSequence;
        private List<Point> psdSequenceReal;
        private List<Point> psdSequenceRealDb;
        private List<Point> autocorrelationSequence;

        private const int stepLength = 10000;
        #endregion
        #region 构造函数
        /// <summary>
        /// 输入参数alpha，beta，画布大小以获取用来绘图的序列
        /// </summary>
        /// <param name="alpha">参数alpha</param>
        /// <param name="beta">参数beta</param>
        public BOC_Sequence_Generate(int alpha, int beta)
        {
            if ((alpha != 0) && (beta != 0))
            {
                if (((2 * (double)alpha) / beta) == (int)((2 * (double)alpha) / beta))
                {
                    bocSequence = BOCSequenceGenerate(alpha);
                    psdSequenceReal = BOCPSDGenerate(alpha, beta);
                    psdSequenceRealDb = ChangeToDb(psdSequenceReal);
                }
                else
                    throw new Exception("2α/β必须为整数");
            }
            else
                throw new Exception("请先输入参数");
        }
        #endregion
#region 方法
        /// <summary>
        /// 返回一个双极性NRZ调制信号
        /// </summary>
        /// <param name="alpha"></param>
        /// <returns></returns>
        private List<Point> BOCSequenceGenerate(int alpha)
        {
            List<Point> points = new List<Point>();
            Random random = new Random();
            double frequence = alpha * 1.023;
            for (int i = 0; i < 20; i++)
            {
                double ak = random.NextDouble();
                if (ak >= 0.5)
                {
                    points.Add(new Point(i / frequence, 1));
                    points.Add(new Point((i + 1) / frequence, 1));
                }
                else
                {
                    points.Add(new Point(i / frequence, -1));
                    points.Add(new Point((i + 1) / frequence, -1));
                }
            }
            return points;
        }

        /// <summary>
        /// 返回一个双极性NRZ调制信号的功率谱密度序列，该序列拥有600个点
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        private List<Point> BOCPSDGenerate(int alpha,int beta)
        {
            List<Point> pointsReal = new List<Point>();//原始的具有正确数值的PSD点序列
            Point point = new Point();//用于添加新点的临时变量
            if(((2 * alpha / beta) % 2) == 0)//若2*alpha/beta等于偶数
            {
                for (int deltaI/*deltaI是为了避免零除点*/, i = -(boundWidth / 2); i <= (boundWidth / 2); i += stepLength)
                {
                    deltaI = i + 1;
                    point.X = i;
                    point.Y = Math.Pow(((Math.Sin(Math.PI * deltaI / (beta * 1.023 * 1000000)) * Math.Tan(Math.PI * deltaI / (2 * alpha * 1.023 * 1000000))) / (Math.PI * deltaI)), 2) * beta * 1.023 * 1000000;
                    pointsReal.Add(point);
                }
            }
            else//若2*alpha/beta等于奇数
            {
                for (int deltaI/*deltaI是为了避免零除点*/, i = -(boundWidth / 2); i <= (boundWidth / 2); i += stepLength)
                {
                    deltaI = i + 1;
                    point.X = i;
                    point.Y = Math.Pow(((Math.Cos(Math.PI * deltaI / (beta * 1.023 * 1000000)) * Math.Tan(Math.PI * deltaI / (2 * alpha * 1.023 * 1000000))) / (Math.PI * deltaI)), 2) * beta * 1.023 * 1000000;
                    pointsReal.Add(point);
                }
            }
            return pointsReal;
        }

        /// <summary>
        /// 由于一些原因，单独做一个函数得到原始的db
        /// </summary>
        /// <param name="pointsReal">原始的PSD序列</param>
        /// <returns></returns>
        private List<Point> ChangeToDb(List<Point> pointsReal)
        {
            List<Point> pointsRealDb = new List<Point>();
            foreach (var point in pointsReal)
            {
                Point tempPoint = new Point(point.X, 10 * Math.Log10(point.Y));
                if (tempPoint.Y >= -100)
                {
                    pointsRealDb.Add(tempPoint);
                }
                else
                    pointsRealDb.Add(new Point(tempPoint.X, -100));
            }
            return pointsRealDb;
        }

        private List<Point> AutocorrelationSequenceGenerate(List<Point> psdSequence)
        {
            double lambda = 0;
            List<Point> rs = new List<Point>();//Rs(t)
            foreach (var point in psdSequence)
            {
                if (point.X >= -12000000 && point.X <= 12000000)  ///带宽β选择24MHz
                {
                    lambda += point.Y * stepLength;
                }
            }
            for (double i = -0.000001; i <= 0.0000010001; i += 0.000000001)
            {
                double tempVal1 = 0;
                double tempVal2 = 0;
                double tempVal3 = 0;
                for (int j = 300; j <= 2700; j++)
                {
                    tempVal1 += psdSequence[j].Y * Math.Cos(2 * Math.PI * psdSequence[j].X * i) * stepLength;
                }
                for (int j = 300; j <= 2700; j++)
                {
                    tempVal2 += psdSequence[j].Y * Math.Sin(2 * Math.PI * psdSequence[j].X * i) * stepLength;
                }
                tempVal3 = Math.Sqrt(Math.Pow(tempVal1, 2) + Math.Pow(tempVal2, 2)) / lambda;
                rs.Add(new Point(i * 1000000, tempVal3));
            }
            return rs;
        }        
        #endregion
        #region 属性
        /// <summary>
        /// 获取一个未经过坐标变换的BOC信号的功率谱密度点序列
        /// </summary>
        public List<Point> GetPsdSequenceReal
        {
            get { return psdSequenceReal; }
        }

        /// <summary>
        /// 获取一个未经过坐标变换的BOC信号的自相关函数点序列
        /// </summary>
        public List<Point> GetAutocorrelationSequence
        {
            get
            {
                if (autocorrelationSequence == null)
                    return autocorrelationSequence = AutocorrelationSequenceGenerate(psdSequenceReal);
                else
                    return autocorrelationSequence;
            }
        }

        /// <summary>
        /// 获取一个经过Db计算的BOC信号的功率谱密度Db点序列
        /// </summary>
        public List<Point> GetPsdSequenceRealDb
        {
            get { return psdSequenceRealDb; }
        }

        /// <summary>
        /// 获取一个双极性非归零码
        /// </summary>
        public List<Point> GetBocSequence
        {
            get { return bocSequence; }
        }
        #endregion
    }
}
