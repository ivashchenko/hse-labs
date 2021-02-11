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
                File.ReadAllLines("../../../tests/test_15.csv")
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

            Stopwatch sw = new Stopwatch();
            while (MaxThreads-- > 1)
            {
                sw.Start();
                var tasks = new List<Task<AlgorithmResult>>();
                for (int n = 0; n < N; n++)
                {
                    int[] nodes = Enumerable.Range(0, N).ToArray();
                    (nodes[n], nodes[0]) = (nodes[0], nodes[n]);

                    tasks.Add(Task.Run(() =>
                    {
                        var result = new AlgorithmResult { Distance = double.MaxValue, Points = nodes.Clone() as int[] };
                        do
                        {
                            double distance = DM[nodes[0], nodes[1]];
                            for (int i = 1; i < nodes.Length; i++)
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

                            if (distance <= result.Distance)
                            {
                                result.Distance = distance;
                                nodes.CopyTo(result.Points, 0);

                                // обновить минимальную длину
                                if (distance < _minPath)
                                    Interlocked.Exchange(ref _minPath, distance);
                            }
                        } while (!NextPermutation<int>(nodes, 1));
                        return result;
                    }));

                    if ((tasks.Count % MaxThreads == 0) || tasks.Count == N)
                        Task.WaitAll(
                            tasks.Where(t => t.Status != TaskStatus.RanToCompletion).ToArray());
                }
                sw.Stop();
                Console.WriteLine($"Threads: {MaxThreads}, time {sw.ElapsedMilliseconds} ms");

                var min = tasks.First(t => t.Result.Distance == tasks.Min(t => t.Result.Distance));
                Console.WriteLine($"{min.Result.Distance} X={string.Join(",", min.Result.Points)}");
            }
        }

        public static bool NextPermutation<T>(T[] elements, int skip=0) where T : IComparable<T>
        {
            // More efficient to have a variable instead of accessing a property
            var count = elements.Length;

            // Indicates whether this is the last lexicographic permutation
            var done = true;

            // Go through the array from last to first
            for (var i = count - 1; i > skip; i--)
            {
                var curr = elements[i];

                // Check if the current element is less than the one before it
                if (curr.CompareTo(elements[i - 1]) < 0)
                {
                    continue;
                }

                // An element bigger than the one before it has been found,
                // so this isn't the last lexicographic permutation.
                done = false;

                // Save the previous (bigger) element in a variable for more efficiency.
                var prev = elements[i - 1];

                // Have a variable to hold the index of the element to swap
                // with the previous element (the to-swap element would be
                // the smallest element that comes after the previous element
                // and is bigger than the previous element), initializing it
                // as the current index of the current item (curr).
                var currIndex = i;

                // Go through the array from the element after the current one to last
                for (var j = i + 1; j < count; j++)
                {
                    // Save into variable for more efficiency
                    var tmp = elements[j];

                    // Check if tmp suits the "next swap" conditions:
                    // Smallest, but bigger than the "prev" element
                    if (tmp.CompareTo(curr) < 0 && tmp.CompareTo(prev) > 0)
                    {
                        curr = tmp;
                        currIndex = j;
                    }
                }

                // Swap the "prev" with the new "curr" (the swap-with element)
                elements[currIndex] = prev;
                elements[i - 1] = curr;

                // Reverse the order of the tail, in order to reset it's lexicographic order
                for (var j = count - 1; j > i; j--, i++)
                {
                    var tmp = elements[j];
                    elements[j] = elements[i];
                    elements[i] = tmp;
                }

                // Break since we have got the next permutation
                // The reason to have all the logic inside the loop is
                // to prevent the need of an extra variable indicating "i" when
                // the next needed swap is found (moving "i" outside the loop is a
                // bad practice, and isn't very readable, so I preferred not doing
                // that as well).
                break;
            }

            // Return whether this has been the last lexicographic permutation.
            return done;
        }
    }
}
