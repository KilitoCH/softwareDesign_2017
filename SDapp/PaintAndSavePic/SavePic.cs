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
        /// <param name="url">保存的路径</param>
        /// <returns>若正常返回true,若发生了异常则返回false</returns>
        public bool SaveVisual(Visual visual,string url)
        {
            try
            {
                RenderTargetBitmap bmp = new RenderTargetBitmap(1000, 500, 96, 96, PixelFormats.Default);//创建一副位图
                bmp.Render(visual);//将visual对象作为位图的内容
                BitmapEncoder encoder = new JpegBitmapEncoder();//新建编码器
                encoder.Frames.Add(BitmapFrame.Create(bmp));//将上述位图导入编码器

                FileStream fileStream = new FileStream(url, FileMode.Create, FileAccess.ReadWrite);//创建并打开一个文件为文件流
                encoder.Save(fileStream);//利用编码器将图像流写入
                fileStream.Close();//关闭文件流
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);//如果出现异常，显示异常信息并返回false
                return false;
            }            
        }

        /// <summary>
        /// 将一个control控件截图保存
        /// </summary>
        /// <param name="control">需要截图的control控件</param>
        /// <param name="url">保存的路径</param>
        /// <returns>是否保存成功</returns>
        public bool SaveControl(Control control,string url)
        {
            try
            {
                DrawingVisual drawingVisual = new DrawingVisual();//创建画板
                using (DrawingContext context = drawingVisual.RenderOpen())//打开一张画布
                {
                    VisualBrush brush = new VisualBrush(control) { Stretch = Stretch.None };//将control作为填充的画笔
                    context.DrawRectangle(brush, null, new Rect(0, 0, control.ActualWidth, control.ActualHeight));//做出一个制定大小的矩形以容纳control
                    context.Close();
                }

                RenderTargetBitmap bmp = new RenderTargetBitmap((int)control.ActualWidth, (int)control.ActualHeight, 96, 96, PixelFormats.Default);//创建一副新的位图
                bmp.Render(drawingVisual);//将前述画布作为位图内容
                BitmapEncoder encoder = new JpegBitmapEncoder();//创建图像流编码器
                encoder.Frames.Add(BitmapFrame.Create(bmp));//将上述位图导入编码器

                FileStream fileStream = new FileStream(url, FileMode.Create, FileAccess.ReadWrite);//创建并打开一个新的文件
                encoder.Save(fileStream);//利用编码器将图像流写入
                fileStream.Close();//关闭文件流
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);//如果出现异常，显示异常信息并返回false
                return false;
            }
        }
    }
}
