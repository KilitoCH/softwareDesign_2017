using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
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
        public bool SaveVisual(Visual visual)
        {
            try
            {
                RenderTargetBitmap bmp = new RenderTargetBitmap(500, 400, 96, 96, PixelFormats.Default);
                bmp.Render(visual);
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));

                FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + "a.jpeg", FileMode.Create, FileAccess.ReadWrite);
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
