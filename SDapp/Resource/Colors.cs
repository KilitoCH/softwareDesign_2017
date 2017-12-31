using System;
using System.Collections.Generic;
using System.Windows.Media;


namespace SoftwareDesign_2017
{
    class Color
    {
        public List<Brush> colors = new List<Brush>();
        
        public Color()
        {
            colors.Add(Brushes.Red);
            colors.Add(Brushes.Green);
            colors.Add(Brushes.Blue);
            colors.Add(Brushes.Brown);
            colors.Add(Brushes.LemonChiffon);
        }
    }
}
