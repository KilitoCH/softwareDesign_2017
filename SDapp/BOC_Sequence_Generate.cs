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
        private List<Point> psdSequenceReal = new List<Point>();
        private List<Point> psdSequenceForView = new List<Point>();
        #endregion
#region 构造函数
        /// <summary>
        /// 输入参数alpha，beta，画布大小以获取用来绘图的序列
        /// </summary>
        /// <param name="alpha">参数alpha</param>
        /// <param name="beta">参数beta</param>
        /// <param name="width">画布的宽</param>
        /// <param name="height">画布的高</param>
        public BOC_Sequence_Generate(int alpha, int beta, double width, double height)
        {
            psdSequenceReal = BOCPSDGenerate(alpha,beta);
            psdSequenceForView = CoordinateTransform(psdSequenceReal, width, height);
        }
#endregion
#region 方法
        /// <summary>
        /// 返回一个双极性NRZ调制信号,注：这个函数尚未完成修改
        /// </summary>
        /// <param name="frequence"></param>
        /// <returns></returns>
        private List<Point> BOC_Sequence(Double frequence)
        {
            List<Point> points = new List<Point>();
            for (int i = -15000000; i <= (boundWidth / 2); i+=1000)
            {
                points.Add(new Point(10 + i * 10 * frequence, i % 2 * 20 + 100));
                points.Add(new Point(10 + (i + 1) * 10 * frequence, i % 2 * 20 + 100));
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
                for (int deltaI/*deltaI是为了避免零除点*/, i = -15000000; i <= (boundWidth / 2); i += 1000)
                {
                    deltaI = i + 1;
                    point.X = i;
                    point.Y = Math.Pow(((Math.Sin(Math.PI * deltaI / (beta * 1.023 * 1000000)) * Math.Tan(Math.PI * deltaI / (2 * alpha * 1.023 * 1000000))) / (Math.PI * deltaI)), 2) * beta * 1.023 * 1000000;
                    pointsReal.Add(point);
                }
            }
            else//若2*alpha/beta等于奇数
            {
                for (int deltaI/*deltaI是为了避免零除点*/, i = -15000000; i <= (boundWidth / 2); i += 1000)
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
        /// 将真实的PSD点阵根据显示空间的大小调整为动态的适合窗口的点阵
        /// </summary>
        /// <param name="pointsReal"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private List<Point> CoordinateTransform(List<Point> pointsReal,double width,double height)
        {
            List<Point> pointsForView = new List<Point>();
            List<Point> pointsTemp = new List<Point>();
            List<Point> pointsRealDb = ChangeToDb(pointsReal);
            Point origin = new Point(width / 2,height);
            double yMax = 0;
            foreach (var point in pointsRealDb)
            {
                pointsTemp.Add(new Point(point.X, point.Y + 100));
                if ((point.Y + 100) > yMax)
                    yMax = point.Y + 100;
            }
            double yScale = origin.Y / yMax;
            double xScale = origin.X / (boundWidth / 2);
            foreach (var point in pointsTemp)
            {
                pointsForView.Add(new Point(point.X * xScale + origin.X, -point.Y * yScale + origin.Y));
            }
            return pointsForView;
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
        #endregion
#region 属性
        /// <summary>
        /// 获取一个未经过坐标变换的BOC信号的功率谱密度点序列
        /// </summary>
        public List<Point> GetPsdSequenceReal
        {
            get { return psdSequenceReal; }
            set { psdSequenceReal = value; }
        }

        /// <summary>
        /// 获取一个坐标变换后，适合画布大小的BOC信号的功率谱密度点序列
        /// </summary>
        public List<Point> GetPsdSequenceForView
        {
            get { return psdSequenceForView; }
            set { psdSequenceForView = value; }
        }
        #endregion
    }
}
