using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace SoftwareDesign_2017
{
    class BOC_Sequence_Generate
    {
        #region 字段
        private int boundWidth;//发射带宽
        private int foreBW;//接收机的前端带宽
        private int stepLength;//发送带宽的量化步进
        private int acCount;//自相关序列的量化步进

        private List<Point> bocSequence;//boc时域序列
        private List<Point> psdSequenceReal;//boc功率谱密度原始序列
        private List<Point> psdSequenceRealDb;//boc功率谱密度db序列，且经过了截断的转换
        private List<Point> autocorrelationSequence;//boc自相关函数序列
        
        #endregion
        #region 构造函数
        /// <summary>
        /// 输入参数alpha，beta，画布大小以获取用来绘图的序列
        /// </summary>
        /// <param name="alpha">参数alpha</param>
        /// <param name="beta">参数beta</param>
        public BOC_Sequence_Generate(int alpha, int beta)
        {
            //从文件中读取预设参数
            FileStream fileStream = new FileStream(@"../../bandWidth.txt", FileMode.Open, FileAccess.Read);//打开参数存储文件并写入文件流
            StreamReader streamReader = new StreamReader(fileStream);//创建文件流写入类
            string[] str = streamReader.ReadLine().Split(',');//从文件中读取字符串，并以','为间隔提取出参数
            boundWidth = Convert.ToInt32(str[0]);//如下四行为将参数存储到本类的私有字段中
            foreBW = Convert.ToInt32(str[1]);
            stepLength = boundWidth / Convert.ToInt32(str[2]);
            acCount = Convert.ToInt32(str[3]);

            if ((alpha != 0) && (beta != 0))///输入的alpha和beta不能为零
            {
                if (((2 * (double)alpha) / beta) == (int)((2 * (double)alpha) / beta))///判断2*alpha/beta是否为整数
                {
                    bocSequence = BOCSequenceGenerate(alpha, beta);///产生BOC信号的时序列
                    psdSequenceReal = BOCPSDGenerate(alpha, beta);///产生BOC信号的功率谱密度的点集合
                    psdSequenceRealDb = ChangeToDb(psdSequenceReal);///把单位W变为dB
                }
                else
                    throw new Exception("2α/β必须为整数");///指定错误
            }
            else
                throw new Exception("请先输入参数");///指定错误
        }
        #endregion
        #region 方法
        /// <summary>
        /// 返回一个双极性NRZ调制信号
        /// </summary>
        /// <param name="alpha"></param>
        /// <returns="points">BOC时序列点集合</returns>
        private List<Point> BOCSequenceGenerate(int alpha, int beta)
        {
            List<Point> points = new List<Point>();
            int n = 2 * alpha / beta;//表示一个扩频码码元周期内的亚载波码元数目
            Random random = new Random();///随机数的产生
            double frequenceC = beta * 1.023;//扩频码的频率
            for (int i = 0; i < 4; i++)
            {
                double ak = random.NextDouble();///随机返回一个大于或等于0.0且小于1.0的随机浮点数
                if (ak >= 0.5) ///判断为大于等于0.5，扩频码电平置1
                {
                    for (int j = 1; j <= n; j++)///在一个扩频码的周期内+1到-1的循环跳变，实现扩频码与亚载波相乘的功能，最终得到最后的伪随机码
                    {
                        if ((j % 2) != 0)///奇数
                        {
                            points.Add(new Point(i / frequenceC + (j - 1) / (n * frequenceC), 1));///每次都以Ts为标准画出一段,取电平为1的两边界点
                            points.Add(new Point(i / frequenceC + j / (n * frequenceC), 1));
                        }
                        else///偶数
                        {
                            points.Add(new Point(i / frequenceC + (j - 1) / (n * frequenceC), -1));///每次都以Ts为标准画出一段,取电平为-1的两边界点
                            points.Add(new Point(i / frequenceC + j / (n * frequenceC), -1));
                        }
                    }
                }
                else///判断为小于0.5，扩频码电平置-1
                {
                    for (int j = 1; j <= n; j++)///同上
                    {
                        if ((j % 2) != 0)///奇数
                        {
                            points.Add(new Point(i / frequenceC + (j - 1) / (n * frequenceC), -1));///同上,-1是扩频码的-1和亚载波的1相乘的结果
                            points.Add(new Point(i / frequenceC + j / (n * frequenceC), -1));
                        }
                        else///偶数
                        {
                            points.Add(new Point(i / frequenceC + (j - 1) / (n * frequenceC), 1));
                            points.Add(new Point(i / frequenceC + j / (n * frequenceC), 1));
                        }
                    }
                }
            }
            return points; ///返回BOC时序列点集合
        }

        /// <summary>
        /// 返回一个双极性NRZ调制信号的功率谱密度序列，该序列拥有600个点
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns="pointsReal">真实数值的PSD点序列</returns>
        private List<Point> BOCPSDGenerate(int alpha, int beta)
        {
            List<Point> pointsReal = new List<Point>();///原始的具有真实数值的PSD点序列
            Point point = new Point();///用于添加新点的临时变量
            if (((2 * alpha / beta) % 2) == 0)///第一张情况，若2*alpha/beta等于偶数
            {
                ///用极小分段的类和代替从-15MHz到+15MHz的积分，stepLength为步长（取10000的步长，一共3000个点）
                for (int deltaI/*deltaI是为了避免零除点*/, i = -(boundWidth / 2); i <= (boundWidth / 2); i += stepLength)
                {
                    deltaI = i + 1;///因为1对于兆的单位很小，所以可以看成极小偏移
                    point.X = i;
                    ///添加新点的y轴坐标为n为偶数情况下BOC的谱密度公式的输出值，其中alpha*1.023*1000000=fs，beta*1.023*1000000=fc
                    point.Y = Math.Pow(((Math.Sin(Math.PI * deltaI / (beta * 1.023 * 1000000)) * Math.Tan(Math.PI * deltaI / (2 * alpha * 1.023 * 1000000))) / (Math.PI * deltaI)), 2) * beta * 1.023 * 1000000;
                    pointsReal.Add(point);///把新点加到真实数值的PSD点序列
                }
            }
            else///第二种情况，若2*alpha/beta等于奇数
            {
                ///用极小分段的类和代替从-15MHz到+15MHz的积分，stepLength为步长（取10000的步长，一共3000个点）
                for (int deltaI/*deltaI是为了避免零除点*/, i = -(boundWidth / 2); i <= (boundWidth / 2); i += stepLength)
                {
                    deltaI = i + 1;///因为1对于兆的单位很小，所以可以看成极小偏移
                    point.X = i;
                    ///添加新点的y轴坐标为n为奇数情况下BOC的谱密度公式的输出值，其中alpha*1.023*1000000=fs，beta*1.023*1000000=fc
                    point.Y = Math.Pow(((Math.Cos(Math.PI * deltaI / (beta * 1.023 * 1000000)) * Math.Tan(Math.PI * deltaI / (2 * alpha * 1.023 * 1000000))) / (Math.PI * deltaI)), 2) * beta * 1.023 * 1000000;
                    pointsReal.Add(point);///把新点加到真实数值的PSD点序列
                }
            }
            return pointsReal;///返回更改后的真实数值的PSD点序列
        }

        /// <summary>
        /// 单独做一个函数做从原始W到原始的dB的转化,并进行了将小于-100db的数据全部截取为-100db的处理
        /// </summary>
        /// <param name="pointsReal">原始的PSD序列</param>
        /// <returns="pointsRealDb">单位为dB的PSD序列</returns>
        private List<Point> ChangeToDb(List<Point> pointsReal)
        {
            List<Point> pointsRealDb = new List<Point>();///定义一个单位为dB的点序列pointsRealDb
            foreach (var point in pointsReal)///依次往真实数据的点序列取点
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
        /// 求BOC信号的归一化自相关函数的点序列
        /// </summary>
        /// <param name="psdSequence">真实的PSD序列</param>
        /// <returns="rs">自相关函数的点阵</returns>
        private List<Point> AutocorrelationSequenceGenerate(List<Point> psdSequence)
        {
            double lambda = 0;///设λ的初始值为0
            List<Point> rs = new List<Point>();///Rs(t)
            int val = (boundWidth - foreBW) / (2 * stepLength);//谱密度序列中取前端带宽的最左一个点
            foreach (var point in psdSequence)///直接利用功率谱密度所求的点集合求λ,为自相关函数归一化做准备
            {
                if (point.X >= -(foreBW / 2) && point.X <= (foreBW / 2))///带宽β选择24MHz
                {
                    lambda += point.Y * stepLength;///用极小分段累加近似代替积分，stepLength为步长（步长取10000.一共取2401个点）
                }
            }
            for (double i = -0.000001; i <= 0.0000010001; i += 0.000002 / acCount)///i为时间t，单位为s，取的点数可自行设置，但范围为-1ns到1ns
            {
                double tempVal1 = 0; ///实部
                double tempVal2 = 0; ///虚部
                double tempVal3 = 0; ///取模
                for (int j = val; j < psdSequence.Count - val; j++)///在psd序列中取到前端带宽占的部分
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
                if (autocorrelationSequence == null)///因为自相关序列计算相对麻烦，所以设置为只有真正被需要的时候才被调用
                    return autocorrelationSequence = AutocorrelationSequenceGenerate(psdSequenceReal);
                else
                    return autocorrelationSequence;
            }
        }

        /// <summary>
        /// 获取一个经过dB计算的BOC信号的功率谱密度dB点序列
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
