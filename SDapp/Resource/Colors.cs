using System;
using System.Collections.Generic;
using System.Windows.Media;


namespace SoftwareDesign_2017
{
    /// <summary>
    /// 画图时可用的颜色
    /// </summary>
    class Color
    {
        //可用的颜色集合
        public List<Brush> colors = new List<Brush>();
        
        public Color()
        {
            colors.Add(Brushes.Green);//绿
            colors.Add(Brushes.MediumBlue);//蓝
            colors.Add(Brushes.Red);//红             
            colors.Add(Brushes.Brown);//棕
            colors.Add(Brushes.LemonChiffon);//柠檬色
        }
    }
}
