using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VoyagerLab1Net
{
    public struct Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double Distance(Point pt)
        {
            return Math.Sqrt(Math.Pow((X - pt.X),2) + Math.Pow((Y - pt.Y),2));
        }
    }

    struct AlgorithmResult
    {
        public double Distance { get; set; }
        public int[] Points { get; set; }
    }

    class ReverseComparer : IComparer<int>
    {
        public int Compare(int p1, int p2)
        {
            return p2 - p1;
        }
    }

    class Program
    {
        static int MaxThreads = Environment.ProcessorCount - 1;
        static double _minPath = double.MaxValue;

        static void Main(string[] args)
        {
            Point[] points =
                File.ReadAllLines("../../../tests/test_16.csv")
                .Select(a => a.Split(';'))
                .Select(xy => new Point
                {
                    X = double.Parse(xy[0], CultureInfo.InvariantCulture),
                    Y = double.Parse(xy[1], CultureInfo.InvariantCulture)
                })
                .ToArray<Point>();

            int N = points.Length;
            double[,] DM = new double[N, N];   // матрица расстояний

            for (int i = 0; i < N; i++)
                for (int j = i + 1; j < N; j++)
                    DM[j, i] = DM[i, j] = points[i].Distance(points[j]);

            //MaxThreads = 1;
            Stopwatch sw = new Stopwatch();
            do
            {
                sw.Start();
                var tasks = new List<Task<AlgorithmResult>>();
                for (int n = 1; n < N; n++)
                {
                    int[] nodes = Enumerable.Range(0, N).ToArray();
                    (nodes[n], nodes[1]) = (nodes[1], nodes[n]);
                    Array.Sort(nodes, 2, nodes.Length - 2);

                    tasks.Add(Task.Run(() =>
                    {
                        var result = new AlgorithmResult { Distance = double.MaxValue, Points = nodes.Clone() as int[] };
                        do
                        {
                            double distance = DM[nodes[0], nodes[1]] + DM[nodes[1], nodes[2]];
                            for (int i = 2; i < nodes.Length; i++)
                            {
                                distance += (i < nodes.Length - 1) ?
                                    DM[nodes[i], nodes[i + 1]] :
                                    DM[nodes[i], nodes[0]]; // возврат в первую точку

                                if (distance > _minPath && i < nodes.Length - 3)
                                {
                                    // дальнейшие перестановки nodes - бесполезны
                                    Array.Sort(nodes, i, nodes.Length - i, new ReverseComparer());
                                    break;
                                }
                            }

                            if (distance < result.Distance)
                            {
                                result.Distance = distance;
                                nodes.CopyTo(result.Points, 0);

                                if (distance < _minPath)
                                    Interlocked.Exchange(ref _minPath, distance);
                            }
                        } while (!NextPermutation<int>(nodes, 2));
                        return result;
                    }));

                    if ((tasks.Count % MaxThreads == 0) || tasks.Count == (N-1))
                        Task.WaitAll(
                            tasks.Where(t => t.Status != TaskStatus.RanToCompletion).ToArray());
                }
                sw.Stop();

                var min = tasks.First(t => t.Result.Distance == tasks.Min(t => t.Result.Distance));
                Console.WriteLine($"Threads: {MaxThreads}, {sw.ElapsedMilliseconds}ms, Min: {min.Result.Distance} path={string.Join(",", min.Result.Points)}");
            } while (--MaxThreads > 0);
        }

        public static bool NextPermutation<T>(T[] elements, int skip=0) where T : IComparable<T>
        {
            var count = elements.Length;
            var done = true;

            for (var i = count - 1; i > skip; i--)
            {
                var curr = elements[i];
                if (curr.CompareTo(elements[i - 1]) < 0)
                    continue;

                done = false;
                var prev = elements[i - 1];
                var currIndex = i;

                for (var j = i + 1; j < count; j++)
                {
                    var tmp = elements[j];
                    if (tmp.CompareTo(curr) < 0 && tmp.CompareTo(prev) > 0)
                    {
                        curr = tmp;
                        currIndex = j;
                    }
                }

                elements[currIndex] = prev;
                elements[i - 1] = curr;

                for (var j = count - 1; j > i; j--, i++)
                {
                    var tmp = elements[j];
                    elements[j] = elements[i];
                    elements[i] = tmp;
                }
                break;
            }

            return done;
        }
    }
}
