using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelbrotLab2Net
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public int Mandelbrot(int depth = 255)
        {
            int i = 0;
            Complex c = new Complex(X, Y), z = new Complex();
            for (; depth > 0 && z.Module2() < 4f; i++, depth--)
            {
                z = z * z + c;
            }
            return i;
        }
    }
}
