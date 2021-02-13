using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MandelbrotLab2Net
{
    public partial class Form : System.Windows.Forms.Form
    {
        Point _origin = new Point { X = -0.5f, Y = 0f };
        float _scale = 250f;

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
        }

        int counter = 0;
        private void OnResize(object sender, EventArgs e)
        {
            this.Text = $"{counter++} {this.pictureBox.Width} {this.pictureBox.Height}";
            this.pictureBox.Image = CreateRandomBitmap(this.pictureBox.Width, this.pictureBox.Height);
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            _scale *= e.Delta > 0 ? 2f : -2f;
            this.Invalidate();
        }
    }
}
