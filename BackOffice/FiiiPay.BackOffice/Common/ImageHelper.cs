using FiiiPay.BackOffice.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiPay.BackOffice.Common
{
    public class ImageHelper
    {
        private const string DefaultFontName = "SimSun";
        private const int DefaultFontSize = 18;
        public static byte[] ImageMerge(params ImageMergeEntity[] imgList)
        {
            if (imgList == null || imgList.Length <= 0)
                return null;
            if (imgList[0].Image == null)
                return null;
            Bitmap bg = new Bitmap(imgList[0].Image);
            Graphics g = Graphics.FromImage(bg);
            Font f;
            Brush b;
            for (int i = 1; i < imgList.Length; i++)
            {
                var imgEntity = imgList[i];
                if (imgEntity.ContentType == ImageContentType.Image)
                {
                    if (imgEntity.Image == null)
                        continue;
                    if (imgEntity.AreaWidth == 0)
                        imgEntity.AreaWidth = imgEntity.Image.Width;
                    if (imgEntity.AreaHeight == 0)
                        imgEntity.AreaWidth = imgEntity.Image.Height;
                    g.DrawImage(imgEntity.Image, new RectangleF(imgEntity.X, imgEntity.Y, imgEntity.AreaWidth, imgEntity.AreaHeight), new RectangleF(0, 0, imgEntity.Image.Width, imgEntity.Image.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    if (string.IsNullOrEmpty(imgEntity.Text))
                        continue;
                    f = new Font(imgEntity.FontName ?? DefaultFontName, imgEntity.FontSize <= 0 ? DefaultFontSize : imgEntity.FontSize, FontStyle.Bold);
                    b = new SolidBrush(imgEntity.FontColor);
                    var sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    g.DrawString(imgEntity.Text, f, b, new RectangleF(imgEntity.X, imgEntity.Y, imgEntity.AreaWidth, imgEntity.AreaHeight), sf);
                }
            }
            g.Dispose();

            using (MemoryStream stream = new MemoryStream())
            {
                bg.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, Convert.ToInt32(stream.Length));
                return data;
            }
        }

        public static void ImageMergeAndSave(string savePath, params ImageMergeEntity[] imgList)
        {
            if (imgList == null || imgList.Length <= 0)
                return;
            if (imgList[0].Image == null)
                return;
            Bitmap bg = new Bitmap(imgList[0].Image);
            Graphics g = Graphics.FromImage(bg);
            Font f;
            Brush b;
            for (int i = 1; i < imgList.Length; i++)
            {
                var imgEntity = imgList[i];
                if (imgEntity.ContentType == ImageContentType.Image)
                {
                    if (imgEntity.Image == null)
                        continue;
                    if (imgEntity.AreaWidth == 0)
                        imgEntity.AreaWidth = imgEntity.Image.Width;
                    if (imgEntity.AreaHeight == 0)
                        imgEntity.AreaWidth = imgEntity.Image.Height;
                    g.DrawImage(imgEntity.Image, new RectangleF(imgEntity.X, imgEntity.Y, imgEntity.AreaWidth, imgEntity.AreaHeight), new RectangleF(0, 0, imgEntity.Image.Width, imgEntity.Image.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    if (string.IsNullOrEmpty(imgEntity.Text))
                        continue;
                    f = new Font(imgEntity.FontName ?? DefaultFontName, imgEntity.FontSize <= 0 ? DefaultFontSize : imgEntity.FontSize, FontStyle.Bold);
                    b = new SolidBrush(imgEntity.FontColor);
                    var sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Center;
                    g.DrawString(imgEntity.Text, f, b, new RectangleF(imgEntity.X, imgEntity.Y, imgEntity.AreaWidth, imgEntity.AreaHeight), sf);
                }
            }
            g.Dispose();
            bg.Save(savePath);
        }
    }
}
