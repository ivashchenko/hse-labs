using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoyagerLab1Net
{
    public class Permutator
    {
        public int[] Array { get; private set; }

        public Permutator(int len)
        {
            Array = new List<int>(Enumerable.Range(1, len)).ToArray();
        }

        public IEnumerable<int> Next(int l = 0)
        {
            if (l == Array.Length)
                yield return 1;
            else
            {
                for (int i = l; i < Array.Length; i++)
                {
                    Swap(l, i);
                    foreach (var _ in Next(l + 1))
                        yield return 1;
                    Swap(l, i);
                }
            }
        }

        void Swap(int i, int j)
        {
            int tmp = Array[i];
            Array[i] = Array[j];
            Array[j] = tmp;
        }
    }
}
