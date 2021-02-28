using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // Исходная функция FFT (использует рекурсивный подход "сверху-вниз")
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

        // параллельная fft, до 2х раз быстрее
        public static Complex[] pfft(Complex[] x, int level = 1, int forkLevel = 1)
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

        // инверсия битов числа int
        public static int Reverse(int value, int high_bit)
        {
            int one = 1, result = 0;

            do
            {
                if ((value & one) > 0)
                    result |= high_bit;
                high_bit >>= 1;
                one <<= 1;
            } while (high_bit != 0);

            return result;
        }

        // FFT снизу-вверх, без рекурсии
        public static Complex[] fft2(Complex[] x)
        {
            int N = x.Length, high_bit = N / 2;
            Complex[] X = new Complex[N];
            Complex[] X2 = new Complex[N];

            for (int n = 2; n <= N; n *= 2)
            {
                for (int i = 0; i < N; i += n)
                {
                    for (int j = i; j < i + n / 2; j++)
                    {
                        if (n == 2)
                        {
                            int jj = Reverse(j, high_bit);
                            int jj1 = Reverse(j + 1, high_bit);
                            X2[j] = x[jj] + x[jj1];
                            X2[j + 1] = x[jj] - x[jj1];
                        }
                        else
                        {
                            X2[j] = X[j] + w(j, n) * X[j + n / 2];
                            X2[j + n / 2] = X[j] - w(j, n) * X[j + n / 2];
                        }
                    }
                }
                (X, X2) = (X2, X);
            }
            return X;
        }

        // параллельная версия FFT снизу-вверх, без рекурсии
        public static Complex[] pfft2(Complex[] x, int maxThreads)
        {
            int N = x.Length, high_bit = N / 2;
            Complex[] X = new Complex[N];
            Complex[] X2 = new Complex[N];

            maxThreads = Math.Min(maxThreads, x.Length / 2);
            int chunk = N / maxThreads;
            Barrier _barrier = new Barrier(maxThreads);

            Parallel.For(0, maxThreads, tid =>
            {

                int start = tid * chunk,
                    end = tid == (maxThreads - 1) ? N : (tid + 1) * chunk;

                for (int n = 2; n <= N; n *= 2)     // размер блока ("бабочки")
                {
                    for (int i = 0; i < N; i += n)  // номер первого элемента в блоке
                    {
                        if (i >= end || (i + n) < start) continue;  // границы потока не пересекают блок. пропускаем!

                        for (int j = i; j < i + n / 2; j++) // индекс элемента блока
                        {
                            if (j < start || j >= end) continue;
                            if (n == 2)
                            {
                                int jj = Reverse(j, high_bit);
                                int jj1 = Reverse(j + 1, high_bit);
                                X2[j] = x[jj] + x[jj1];
                            }
                            else
                                X2[j] = X[j] + w(j, n) * X[j + n / 2];
                        }

                        for (int j = i + n / 2; j < i + n; j++)
                        {
                            if (j < start || j >= end) continue;
                            if (n == 2)
                            {
                                int jj = Reverse(j, high_bit);
                                int jj1 = Reverse(j - 1, high_bit);
                                X2[j] = x[jj1] - x[jj];
                            }
                            else
                                X2[j] = X[j - n / 2] - w(j - n / 2, n) * X[j];
                        }
                    }
                    _barrier.SignalAndWait();   // дождаться пока ВСЕ потоки закончат этот уровень                    
                    if (tid == 0)
                        (X, X2) = (X2, X);
                    _barrier.SignalAndWait();   // ещё раз подождать
                }
            });

            return X;
        }
    }
}
