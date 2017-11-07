using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace SoftwareDesign_2017
{
    class Save
    {
        public bool SaveVisual(Visual visual,int width,int height)
        {
            RenderTargetBitmap bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            bmp.Render(visual);
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            
            /*byte[] buff = resourceClient.DownLoadImage(path);
            BitmapImage bmp = new BitmapImage();
            if (buff != null)
            {
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(buff);
                bmp.EndInit();
            }
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            */

            FileStream fileStream = new FileStream(Directory.GetCurrentDirectory() + "a.jpeg", FileMode.Create, FileAccess.ReadWrite);
            encoder.Save(fileStream);
            fileStream.Close();
            return true;
        }
    }
}
