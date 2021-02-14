using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelbrotLab2Net
{
    public struct Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public bool IsMandelbrot(int depth = 100)
        {
            int i = 0;
            Complex c = new Complex(X, Y), z = new Complex();
            for (; i < depth && z.Module() < 2f; i++)
            {
                z = z * z + c;
            }
            return i == depth;
        }
    }
}
