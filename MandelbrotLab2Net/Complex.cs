using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotLab2Net
{
    public class Complex
    {
        public double Re { get; private set; }
        public double Im { get; private set; }

        public Complex(double re = 0, double im = 0)
        {
            Re = re;
            Im = im;
        }

        public float Module()
        {
            return (float)Math.Sqrt(Re * Re + Im * Im);
        }
        
        public double Module2()
        {
            return Re * Re + Im * Im;
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
}
