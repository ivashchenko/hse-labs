using System;
using System.Numerics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using System.Text;

namespace FftLab2
{
    class Program
    {
        static double error(Complex[] signal1, Complex[] signal2)
        {
            double res = 0;
            for (int i = 0; i < signal1.Length; i++)
            {
                double e = (signal1[i] - signal2[i]).Magnitude;
                res += e * e;
            }
            return Math.Sqrt(res / signal1.Length);
        }

        static void Main(string[] args)
        {
            int P = 24, N = (int)Math.Pow(2, P);
            Stopwatch sw = new Stopwatch();
            Complex[] signal = new Complex[N];

            for (int t = 0; t < signal.Length; t++)
                signal[t] = Math.Sin(2 * Math.PI * t / (N / 50)) + Math.Sin(2 * Math.PI * t / (N / 100)) / 2;
            //signal[t] = t % 100;

            sw.Restart();
            Complex[] sp1 = FFT.fft(signal);
            sw.Stop();
            Console.WriteLine($"1'st fft(N=2^{P}): {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            int forkLevel = (int)Math.Log2(Environment.ProcessorCount);
            var sp2 = FFT.pfft(signal, 1, forkLevel);
            sw.Stop();
            Console.WriteLine($"2'd pfft: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine($"Signals are equal: {sp1.SequenceEqual(sp2)}, error={error(sp1, sp2)}");

            sw.Restart();
            Complex[] sp3 = FFT.fft2(signal);
            sw.Stop();
            Console.WriteLine($"3'd fft2(N=2^{P}): {sw.ElapsedMilliseconds} ms");
            Console.WriteLine($"Signals are equal: {sp1.SequenceEqual(sp3)}, error={error(sp1, sp3)}");

            Console.WriteLine($"4'th pfft2 loop:");
            for (int i = 1; i <= Environment.ProcessorCount; i++)
            {
                sw.Restart();
                Complex[] sp4 = FFT.pfft2(signal, i);
                sw.Stop();
                Console.WriteLine($"Threads={i} done {sw.ElapsedMilliseconds} ms, signals are equal: {sp1.SequenceEqual(sp2)}, error = {error(sp3, sp4)}");
            }

            if (P < 10)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < N; i++)
                    sb.AppendLine($"{signal[i].Real};{sp1[i].Magnitude};{sp3[i].Magnitude}");
                System.IO.File.WriteAllText("fft-analysis.csv", sb.ToString());
            }
        }
    }
}
