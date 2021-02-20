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

        public static Task<Complex[]> pfft(Complex[] x)
        {
            return Task.Run(() => {
                //Console.WriteLine($"{x.Length}");
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
                    
                    var taskEven = pfft(x_even);
                    var taskOdd = pfft(x_odd);
                    Task.WaitAll(taskEven, taskOdd);
                    Complex[] X_even = taskEven.Result;
                    Complex[] X_odd = taskOdd.Result;

                    X = new Complex[N];
                    for (int i = 0; i < N / 2; i++)
                    {
                        X[i] = X_even[i] + w(i, N) * X_odd[i];
                        X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                    }
                }
                return X;

            });
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
