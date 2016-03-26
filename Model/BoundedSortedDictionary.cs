using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;


namespace RecommenderSystem
{
    public class BoundedSortedList<K, V> where V : IComparable<V>
    {
        private SortedSet<Pair<K, V>> sortedSet;
        private int MAX_SIZE;

        public BoundedSortedList(int maxSize)
        {
            this.MAX_SIZE = maxSize;
            this.sortedSet = new SortedSet<Pair<K, V>>();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void add(K key, V value)
        {
            if (sortedSet.Count < MAX_SIZE)
            {
                sortedSet.Add(new Pair<K, V>(key, value));
                return;
            }

            if (value.CompareTo(sortedSet.First().v) > 0)
            {
                sortedSet.Remove(sortedSet.First());
                add(key, value);
            }

            return;
        }

        public List<KeyValuePair<K, V>> getList()
        {
           List<Pair<K, V>> list = sortedSet.ToList();
           return list.ConvertAll(pair => new KeyValuePair<K, V>(pair.k, pair.v));
        }

        private class Pair<K, V> : IComparable<Pair<K, V>> where V : IComparable<V>
        {
            public K k;
            public V v;

            public Pair(K k, V v)
            {
                this.k = k;
                this.v = v;
            }

            public int CompareTo(Pair<K, V> that)
            {
                return this.v.CompareTo(that.v);
            }
        }

    }
}

