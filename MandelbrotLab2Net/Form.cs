using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotLab2Net
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap bitmap;
        Point C;           // center 0,0
        double K = 1.0/128.0;    // scale
        int MaxThreads = Environment.ProcessorCount - 1;

        Point toCanvasXY(double x, double y)
        {
            return new Point { X = C.X + x / K, Y = C.Y - y / K };
        }

        Point fromCanvasXY(double X, double Y)
        {
            return new Point { X = (X - C.X) * K, Y = (C.Y - Y) * K };
        }

        Color get_rgb_smooth(int n, int iter_max)
        {
            double t = (double)n / (double)iter_max;
            int r = (int)(9 * (1 - t) * t * t * t * 255);
            int g = (int)(15 * (1 - t) * (1 - t) * t * t * 255);
            int b = (int)(8.5 * (1 - t) * (1 - t) * (1 - t) * t * 255);
            return Color.FromArgb(r, g, b);
         }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            if (bitmap == null)
                bitmap = new Bitmap(e.ClipRectangle.Width, e.ClipRectangle.Height, PixelFormat.Format24bppRgb);

            float W = bitmap.Width, H = bitmap.Height;

            unsafe
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;

                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0, _x = 0; x < bitmapData.Width; x += 1, _x += bytesPerPixel)
                    {
                        Point pt = fromCanvasXY(x, y);

                        int id = pt.Mandelbrot(150);
                        Color c = get_rgb_smooth(id, 150);
                        currentLine[_x] = c.B;
                        currentLine[_x + 1] = c.G;
                        currentLine[_x + 2] = c.R;
                    }
                });
                bitmap.UnlockBits(bitmapData);
            }

            e.Graphics.DrawImage(bitmap, 0, 0);

            if(1/K < 1000)
                for (int x = -3; x <= 2; x++)
                    for (int y = -2; y <= 2; y++)
                    {
                        Point pt = toCanvasXY(x, y);
                        e.Graphics.FillRectangle(x == 0 && y == 0 ? Brushes.Red : Brushes.Yellow, (float)pt.X - 1, (float)pt.Y - 1, 3, 3);
                    }

            e.Graphics.Dispose();
            this.Text = $"Увеличение {1.0 / K} ";
        }

        public Form()
        {
            InitializeComponent();
            this.MouseWheel += PictureBox_MouseWheel;
            this.SizeChanged += OnResize;
            this.Paint += Form_Paint;

            C = new Point { X = this.Width * 0.75f, Y = this.Height / 2f };            
        }

        private void OnResize(object sender, EventArgs e)
        {
            bitmap = null;
            this.Invalidate(true);
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            double n = e.Delta > 0 ? 0.25 : 4.0;
            K *= n;
            C.X = (n - 1) * e.X / n + C.X / n;
            C.Y = (n - 1) * e.Y / n + C.Y / n;
            this.Invalidate(true);
        }

        private void Form_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            K = 1.0 / 128.0;
            C = new Point { X = this.Width * 0.75f, Y = this.Height / 2f };
            this.Invalidate(true);
        }
    }
}
