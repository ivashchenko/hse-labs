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
                    (Array[l], Array[i]) = (Array[i], Array[l]);
                    foreach (var _ in Next(l + 1))
                        yield return 1;
                    (Array[l], Array[i]) = (Array[i], Array[l]);
                }
            }
        }
    }
}
