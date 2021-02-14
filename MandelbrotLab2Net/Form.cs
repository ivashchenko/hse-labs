using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MandelbrotLab2Net
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap _drawArea;
        float K = 3f/300;    // scale
        Point C;           // center 0,0

        Point toCanvasXY(float x, float y)
        {
            return new Point { X = C.X + x / K, Y = C.Y - y / K };
        }

        private void Form_Paint(object sender, PaintEventArgs e)
        {
            if (this.pictureBox.Image == null) return;

            Graphics g = Graphics.FromImage(_drawArea);
            Pen mypen = new Pen(Brushes.Black, 3);
            Pen penCirle = new Pen(Brushes.Blue, 1), penZero = new Pen(Brushes.Red, 2); ;
            g.Clear(Color.White);
            //g.DrawLine(mypen, 0, 0, 200, 200);
            float W = this.pictureBox.Width, H = this.pictureBox.Height;

            float xmin = -C.X * K, xmax = (W - C.X) * K;
            float ymin = (C.Y - H) * K, ymax = C.Y * K;

            for (int x = -5; x < 5; x++)
                for (int y = -5; y < 5; y++)
                {
                    Point pt = toCanvasXY(x, y);
                    g.DrawEllipse(x == 0 && y == 0 ? penZero : penCirle, pt.X - 2, pt.Y - 2, 4, 4);
                }

            g.Dispose();
        }

        private Bitmap CreateRandomBitmap(int width, int height)
        {
            var randomPixels = new byte[4 * width * height];
            new Random().NextBytes(randomPixels);
            var gch = GCHandle.Alloc(randomPixels, GCHandleType.Pinned);
            IntPtr dataPtr = gch.AddrOfPinnedObject();
            var randomBmp = new Bitmap(width, height, width * 4, PixelFormat.Format32bppArgb, dataPtr);
            gch.Free();
            return randomBmp;
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
            if (this.pictureBox.Width > 0 && this.pictureBox.Height > 0)
            {
                _drawArea = CreateRandomBitmap(this.pictureBox.Width, this.pictureBox.Height);
                this.pictureBox.Image = _drawArea;
            }
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
