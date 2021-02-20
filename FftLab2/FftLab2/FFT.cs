using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FftLab2
{
    // https://ru.wikibooks.org/wiki/
    public class FFT
    {
        private static Complex w(int k, int N)
        {
            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }

        public static Complex[] fft(Complex[] x)
        {
            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                Complex[] X_even = fft(x_even);
                Complex[] X_odd = fft(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + w(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                }
            }
            return X;
        }

        public static Complex[] pfft(Complex[] x, byte type = 0)
        {
            Complex[] X;
            int N = x.Length;

            Complex[] x_even = new Complex[N / 2];
            Complex[] x_odd = new Complex[N / 2];
            for (int i = 0; i < N / 2; i++)
            {
                x_even[i] = x[2 * i];
                x_odd[i] = x[2 * i + 1];
            }
            Complex[] X_even = null, X_odd = null;
            //Console.WriteLine($"N={N}, {type}");
            if (N == 4)
            {
                X_even = new Complex[2];
                X_even[0] = x_even[0] + x_even[1];
                X_even[1] = x_even[0] - x_even[1];
                X_odd = new Complex[2];
                X_odd[0] = x_odd[0] + x_odd[1];
                X_odd[1] = x_odd[0] - x_odd[1];
            }
            else
            {
                var t1 = Task.Run(() => pfft(x_even, 2));
                var t2 = Task.Run(() => pfft(x_odd, 1));
                Task.WaitAll(t1, t2);
                X_even = t1.Result;
                X_odd = t2.Result;
            }
            X = new Complex[N];
            for (int i = 0; i < N / 2; i++)
            {
                X[i] = X_even[i] + w(i, N) * X_odd[i];
                X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
            }
            return X;
        }

        public static Complex[] nfft(Complex[] X)
        {
            int N = X.Length;
            Complex[] X_n = new Complex[N];
            for (int i = 0; i < N / 2; i++)
            {
                X_n[i] = X[N / 2 + i];
                X_n[N / 2 + i] = X[i];
            }
            return X_n;
        }
    }
}
