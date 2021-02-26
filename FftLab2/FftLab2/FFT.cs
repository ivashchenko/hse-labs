using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
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

        static int counter = 0;

        public static Complex[] pfft(Complex[] x, int level, int forkLevel)
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
                Complex[] X_even = null, X_odd = null;

                if (level < forkLevel)
                {
                    Parallel.Invoke(
                        () => X_even = pfft(x_even, level + 1, forkLevel),
                        () => X_odd = pfft(x_odd, level + 1, forkLevel));
                }
                else
                {
                    X_even = fft(x_even);
                    X_odd = fft(x_odd);
                }

                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + w(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                }
            }
            return X;
        }

        public static Complex[] p2fft(Complex[] x, int maxThreads, int level = 1)
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
                Complex[] X_even = null, X_odd = null;
                if (false)
                {
                    X_even = p2fft(x_even, maxThreads, level + 1);
                    X_odd = p2fft(x_odd, maxThreads, level + 1);

                    Parallel.For(0, maxThreads, p =>
                    {
                        int n = N / 2;
                        int M = n / maxThreads, m = n % maxThreads;
                        int max = p == maxThreads - 1 ? M + m : M;

                        for (int j = 0, i = p * M; j < max; j++, i++)
                        {
                            X[i] = X_even[i] + w(i, N) * X_odd[i];
                            X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                        }
                    });
                }
                else
                {
                    int myId = 0;
                    if (level < 4) 
                    {
                        myId = Interlocked.Increment(ref counter);
                        Console.WriteLine($"{myId} ({level}) born {DateTime.Now.Second}:{DateTime.Now.Millisecond}");
                    }

                    if (level <= 2)
                    {
                        int nextlevel = level + 1;
                        Parallel.Invoke(
                            () => X_even = p2fft(x_even, maxThreads, nextlevel),
                            () => X_odd = p2fft(x_odd, maxThreads, nextlevel));

                        X = new Complex[N];

                        //Console.WriteLine("done");
                        Parallel.For(0, 2, p =>
                        {
                            int n = N / 2;
                            int M = n / 2;

                            for (int j = 0, i = p * M; j < M; j++, i++)
                            {
                                X[i] = X_even[i] + w(i, N) * X_odd[i];
                                X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                            }
                        });
                    }
                    else
                    {
                        int nextlevel = level + 1;
                        if (level == 3)
                            Console.WriteLine($"n={N}");

                        //X_even = p2fft(x_even, maxThreads, nextlevel);
                        //X_odd = p2fft(x_odd, maxThreads, nextlevel);

                        X_even = fft(x_even);
                        X_odd = fft(x_odd);

                        X = new Complex[N];
                        for (int i = 0; i < N / 2; i++)
                        {
                            X[i] = X_even[i] + w(i, N) * X_odd[i];
                            X[i + N / 2] = X_even[i] - w(i, N) * X_odd[i];
                        }
                    }
                    string space = " ";
                    if(myId > 0)
                        Console.WriteLine($"{space:level}{myId} dead {DateTime.Now.Second}:{DateTime.Now.Millisecond}");
                }
            }
            return X;
        }
    }
}
