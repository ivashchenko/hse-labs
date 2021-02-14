using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MandelbrotLab2Net
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap _bitmap;
        float K = 3f/300;    // scale
        Point C;           // center 0,0

        Point toCanvasXY(float x, float y)
        {
            return new Point { X = C.X + x / K, Y = C.Y - y / K };
        }

        Point fromCanvasXY(float X, float Y)
        {
            return new Point { X = (X - C.X) * K, Y = (C.Y - Y) * K };
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            if (this.pictureBox.Image == null)
            {
                if (this.pictureBox.Width > 0 && this.pictureBox.Height > 0)
                {
                    _bitmap = new Bitmap(this.pictureBox.Width, this.pictureBox.Height, PixelFormat.Format32bppArgb);
                    this.pictureBox.Image = _bitmap;
                }
                else
                    return;
            }

            Graphics g = Graphics.FromImage(_bitmap);

            Pen mypen = new Pen(Brushes.Black, 3);
            Pen penCirle = new Pen(Brushes.Blue, 1), penZero = new Pen(Brushes.Red, 2); ;
            g.Clear(Color.White);
            //g.DrawLine(mypen, 0, 0, 200, 200);

            /*
            for (int j = 0; j < _bitmap.Height; j++)
                for (int i = 0; i < _bitmap.Width; i++)
                    _bitmap.SetPixel(i, j, Color.BlueViolet);
            */

            BitmapData bmd = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
                                  System.Drawing.Imaging.ImageLockMode.ReadWrite,
                                  _bitmap.PixelFormat);

            int PixelSize = 4;
            unsafe
            {
                for (int y = 0; y < bmd.Height; y++)
                {
                    byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);

                    for (int x = 0; x < bmd.Width; x++)
                    {
                        Point pt = fromCanvasXY(x, y);
                        byte b = 255;
                        if (pt.IsMandelbrot()) 
                            b = 0;

                        row[x * PixelSize] = b;   //Blue  0-255
                        row[x * PixelSize + 1] = b; //Green 0-255
                        row[x * PixelSize + 2] = b;   //Red   0-255
                        row[x * PixelSize + 3] = 200;  //Alpha 0-255
                    }
                }
            }

            _bitmap.UnlockBits(bmd);

            float W = this.pictureBox.Width, H = this.pictureBox.Height;

            float xmin = -C.X * K, xmax = (W - C.X) * K;
            float ymin = (C.Y - H) * K, ymax = C.Y * K;

            for (int x = -5; x < 5; x++)
                for (int y = -5; y < 5; y++)
                {
                    Point pt = toCanvasXY(x, y);
                    g.DrawEllipse(x == 0 && y == 0 ? penZero : penCirle, pt.X - 2, pt.Y - 2, 4, 4);
                }
            Point pt1 = fromCanvasXY(0, 0), pt2 = fromCanvasXY(W, H);

            g.DrawString($"{pt1.X} - {pt1.Y}", DefaultFont, Brushes.Green, new PointF(0, 0));
            g.DrawString($"{pt2.X} - {pt2.Y}", DefaultFont, Brushes.Green, new PointF(W-50, H-20));
            g.Dispose();
        }

        public Form()
        {
            InitializeComponent();
            this.pictureBox.MouseWheel += PictureBox_MouseWheel;
            this.pictureBox.SizeChanged += OnResize;
            this.Paint += Form_Paint;

            C = new Point { X = this.pictureBox.Width * 0.75f, Y = this.pictureBox.Height / 2f };            
        }

        int counter = 0;
        private void OnResize(object sender, EventArgs e)
        {
            this.Text = $"{counter++} {this.pictureBox.Width} {this.pictureBox.Height}";
            this.pictureBox.Image = null;
            //this.Invalidate(true);
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            float n = e.Delta > 0 ? 0.5f : 2f;
            K *= n;
            C.X = (n - 1) * e.X / n + C.X / n;
            C.Y = (n - 1) * e.Y / n + C.Y / n;
            this.Invalidate(true);
        }
    }
}
