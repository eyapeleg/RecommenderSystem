using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace RecommenderSystem
{
    public class BoundedSortedCollection<T> where T : IComparable<T>
    {
           
        private SortedSet<T> sortedSet;
        private int MAX_SIZE;

        public BoundedSortedCollection(int maxSize)
        {
            this.MAX_SIZE = maxSize;
            this.sortedSet = new SortedSet<T>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void add(T t)
        {
            if (sortedSet.Count < MAX_SIZE)
            {
                sortedSet.Add(t);
                return;
            }

            if (t.CompareTo(sortedSet.First()) > 0)
            {
                sortedSet.Remove(sortedSet.First());
                add(t);
            }

            return;
        }

        public List<T> AsList()
        {
            return this.sortedSet.ToList();
        }

    }
}
