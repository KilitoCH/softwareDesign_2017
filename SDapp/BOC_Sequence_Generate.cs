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
        const int SEQUENCE_LENGTH = 100;

        /// <summary>
        /// 返回一个双极性NRZ调制信号
        /// </summary>
        /// <param name="frequence"></param>
        /// <returns></returns>
        public List<Point> BOC_Sequence(Double frequence)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < SEQUENCE_LENGTH; i++)
            {
                points.Add(new Point(10 + i * 10 * frequence, i % 2 * 20 + 100));
                points.Add(new Point(10 + (i + 1) * 10 * frequence, i % 2 * 20 + 100));
            }
            return points;
        }

        public List<Point> BOC_PSD_Sequence(Double frequence)
        {
            List<Point> points = new List<Point>();
            Point point = new Point();
            points.Add(new Point(10, 200));
            for (int i = 1; i < SEQUENCE_LENGTH; i++)
            {
                if (i != 40)
                {
                    point.X = i * 5 + 10;
                    point.Y = -Math.Pow(((Math.Sin(Math.PI * (i - 40) / (10 * frequence)) * Math.Tan(Math.PI * (i - 40) / (40 * frequence))) / (Math.PI * (i - 40) / (10 * frequence))), 2) * 150 + 200;
                    points.Add(point);
                }
                else
                    points.Add(new Point(210, -Math.Pow(((Math.Sin(Math.PI * 0.000000001 / (10 * frequence)) * Math.Tan(Math.PI * 0.000000001 / (40 * frequence))) / (Math.PI * 0.000000001 / (10 * frequence))), 2) * 150 + 200));
            }
            return points;
        }
    }
}
