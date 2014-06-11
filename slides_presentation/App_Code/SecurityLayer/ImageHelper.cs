using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ImageHelper
/// </summary>
public class ImageHelper
{

    public static Image b64ToImage(string b64)
    {
        Byte[] ibytes = Convert.FromBase64String(b64);
        MemoryStream m = new MemoryStream(ibytes, 0, ibytes.Length);
        m.Write(ibytes, 0, ibytes.Length);
        Image img = Image.FromStream(m, true);
        return img;
    }

    public static Image cropImage(Image img, Rectangle area)
    {
        var bmp = new Bitmap(img);
        Bitmap bmpCrop = bmp.Clone(area, PixelFormat.Format16bppRgb555);
        return (Image)bmpCrop;
    }

    public static Rectangle ratioRectangle(Rectangle area, decimal ratio)
    {
        return new Rectangle() { X = Convert.ToInt32(area.X * ratio), Y = Convert.ToInt32(area.Y * ratio), Width = Convert.ToInt32(area.Width * ratio), Height = Convert.ToInt32(area.Height * ratio) };
    }

    public static Image resize(Image o_img, int width)
    {
        Bitmap m_img = null;

        using (var bm = new Bitmap(o_img))
        {
            int w = width;
            int h = Convert.ToInt32(w * bm.Height / Convert.ToDecimal(bm.Width));
            m_img = new Bitmap(w, h);

            using (Graphics g = Graphics.FromImage(m_img))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bm, new Rectangle(0, 0, w, h), new Rectangle(0, 0, bm.Width, bm.Height), GraphicsUnit.Pixel);
            }
        }
        return m_img;
    }
}