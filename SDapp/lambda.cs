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
            const double beta = 24000000;
            double lambda = 0;
            foreach (var point in points)
            {
                lambda += point.Y * 0.01;
            }
            return lambda;
        }
    }
}
