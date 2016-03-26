using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;


namespace RecommenderSystem
{
    public class BoundedSortedList<K, V> where K : IComparable<K> 

    {
        private SortedList<K, V> sortedList;
        private int MAX_SIZE;

        public BoundedSortedList(int maxSize)
        {
            this.MAX_SIZE = maxSize;
            this.sortedList = new SortedList<K,V>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void add(K key, V value)
        {
            if (sortedList.Count < MAX_SIZE)
            {
                sortedList.Add(key, value);
                return;
            }

            if (key.CompareTo(sortedList.Keys.First()) > 0)
            {
                sortedList.Remove(sortedList.Keys.First());
                add(key,value);
            }

            return;
        }

        public List<KeyValuePair<K, V>> getList()
        {
            return sortedList.ToList();
        }

    }
}

