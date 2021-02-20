using System;
using System.Numerics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FftLab2
{
    class Program
    {
        static void Main(string[] args)
        {
            int N = (int)Math.Pow(2, 25);
            Complex[] signal = new Complex[N];

            Console.WriteLine($"Signal N={N}");

            for (int t = 0; t < signal.Length; t++)
                signal[t] = Math.Sin(2 * Math.PI * t / 1000);

            Stopwatch sw = new Stopwatch();
            sw.Restart();
            var spectre1 = FFT.fft(signal);
            sw.Stop();
            Console.WriteLine($"1'st: {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            var spectre2 = FFT.pfft(signal);
            sw.Stop();
            Console.WriteLine($"2'd: {sw.ElapsedMilliseconds} ms");

            Console.WriteLine($"Signals are equal: {spectre1.SequenceEqual(spectre2)}");
        }
    }
}
