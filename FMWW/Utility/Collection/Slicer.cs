using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FMWW.Utility.Collection
{
    public class Slicer<T>
    {
        public static List<List<T>> Cut(ICollection<T> xs, int n)
        {
            var nestedList = new List<List<T>>() { new List<T>() };
            int unit = xs.Count / n;
            if (unit == 0)
            {
                nestedList[0].AddRange(xs);
                return nestedList;
            }

            int i = 0, j = 0;
            foreach (var x in xs)
            {
                if (i >= unit * n)
                {
                    nestedList[j++].Add(x);
                    continue;
                }

                var end = nestedList.Count - 1;
                if (nestedList[end].Count < unit)
                {
                    nestedList[end].Add(x);
                }
                else
                {
                    nestedList.Add(new List<T>() { x });
                }
                i++;
            }
            return nestedList;
        }

        public static List<List<T>> Cut(ICollection<T> xs)
        {
#if true
            return Cut(xs, Environment.ProcessorCount);
#else
            var nestedList = new List<List<T>>() { new List<T>() };
            int unit = xs.Count / Environment.ProcessorCount;
            if (unit == 0)
            {
                nestedList[0].AddRange(xs);
                return nestedList;
            }

            int i = 0, j = 0;
            foreach (var x in xs)
            {
                if (i >= unit * Environment.ProcessorCount)
                {
                    nestedList[j++].Add(x);
                    continue;
                }

                var end = nestedList.Count - 1;
                if (nestedList[end].Count < unit)
                {
                    nestedList[end].Add(x);
                }
                else
                {
                    nestedList.Add(new List<T>() { x });
                }
                i++;
            }
            return nestedList;
#endif
        }
    }
}
