using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace SoftwareDesign_2017
{
    class SavePic
    {
        /// <summary>
        /// 用于将一个visual对象输出为一张jpeg格式图片
        /// </summary>
        /// <param name="visual">需要进行转换的visual对象</param>
        /// <returns>若正常返回true,若发生了异常则返回false</returns>
        public bool SaveVisual(Visual visual,string url)
        {
            try
            {
                RenderTargetBitmap bmp = new RenderTargetBitmap(428, 248, 96, 96, PixelFormats.Default);
                bmp.Render(visual);
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));

                FileStream fileStream = new FileStream(url, FileMode.Create, FileAccess.ReadWrite);
                encoder.Save(fileStream);
                fileStream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public bool SaveControl(Control control,string url)
        {
            try
            {
                DrawingVisual drawingVisual = new DrawingVisual();
                using (DrawingContext context = drawingVisual.RenderOpen())
                {
                    VisualBrush brush = new VisualBrush(control) { Stretch = Stretch.None };
                    context.DrawRectangle(brush, null, new Rect(0, 0, control.Width, control.Height));
                    context.Close();
                }

                RenderTargetBitmap bmp = new RenderTargetBitmap((int)control.ActualWidth + 50, (int)control.ActualHeight + 50, 96, 96, PixelFormats.Default);
                bmp.Render(drawingVisual);
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));

                FileStream fileStream = new FileStream(url, FileMode.Create, FileAccess.ReadWrite);
                encoder.Save(fileStream);
                fileStream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
