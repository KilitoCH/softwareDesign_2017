using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SoftwareDesign_2017
{
    static class Lambda
    {
        static public double getLambda(List<Point> points)
        {
            double lambda = 0;
            foreach (var point in points)
            {
                if (point.X >= -12000000 && point.X <= 12000000)
                {
                    lambda += point.Y * 1000;
                }                
            }
            return lambda;
        }
    }
}
