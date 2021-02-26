using System;
using System.Numerics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace FftLab2
{
    class Program
    {
        static int counter = 0;

        private static Complex w(int k, int N)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }

        static void Main(string[] args)
        {
            int N = (int)Math.Pow(2, 20);
            Stopwatch sw = new Stopwatch();

            Complex[] signal = new Complex[N];

            for (int t = 0; t < signal.Length; t++)
                signal[t] = Math.Sin(2 * Math.PI * t / 1000);

            sw.Restart();
            var spectre1 = FFT.fft(signal);
            sw.Stop();
            Console.WriteLine($"1'st: {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            int forkLevel = (int)Math.Log2(8);
            var spectre2 = FFT.p2fft(signal, 4);
            sw.Stop();
            Console.WriteLine($"2'd: {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            var spectre3 = FFT.pfft(signal, 1, forkLevel);
            sw.Stop();
            Console.WriteLine($"3'd: {sw.ElapsedMilliseconds} ms");

            Console.WriteLine($"Signals are equal: {spectre1.SequenceEqual(spectre2)}");
        }
    }
}
