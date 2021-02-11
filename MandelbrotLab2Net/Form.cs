using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotLab2Net
{
    struct Point 
    { 
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class Complex
    {
        public float Re { get; private set; }
        public float Im { get; private set; }

        public Complex(float re, float im)
        {
            Re = re;
            Im = im;
        }

        public float Module()
        {
            return (float)Math.Sqrt(Re * Re + Im * Im);
        }

        public static Complex operator +(Complex c1, Complex c2) 
        {
            return new Complex(c1.Re + c2.Re, c1.Im + c2.Im);
        }

        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex(c1.Re * c2.Re - c1.Im * c2.Im, c1.Re * c2.Im + c1.Im * c2.Re);
        }
    }

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
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            _scale *= e.Delta > 0 ? 2f : -2f;
            this.Invalidate();
        }
    }
}
