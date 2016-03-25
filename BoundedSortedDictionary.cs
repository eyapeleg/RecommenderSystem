using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    class BoundedSortedDictionary<T, U> :IEnumerable<KeyValuePair<T, U>> where T : IComparable<T> 

    {
        private SortedDictionary<T, U> sortedDictionary;
        private int MAX_SIZE;

        public BoundedSortedDictionary(int maxSize)
        {
            this.MAX_SIZE = maxSize;
            this.sortedDictionary = new SortedDictionary<T, U>();
        }

        public void add(T key, U value)
        {
            KeyValuePair<T, U> keyValuePair = new KeyValuePair<T, U>(key,value);
            add(keyValuePair);
        }

        public void add(KeyValuePair<T,U> keyValuePair){
            if (sortedDictionary.Count() < MAX_SIZE)
            {
                sortedDictionary.Add(keyValuePair.Key, keyValuePair.Value);
                return;
            }

            if (keyValuePair.Key.CompareTo(sortedDictionary.Keys.First()) > 0 )
            {
                sortedDictionary.Remove(sortedDictionary.Keys.First());
                add(keyValuePair);
            }

            return;
        }

        public IEnumerator<KeyValuePair<T, U>> GetEnumerator()
        {
            return sortedDictionary.GetEnumerator();
        }

        /*public IEnumerable<KeyValuePair<T, U>> getEnumerable()
        {
            return this.sortedDictionary.AsEnumerable();
        }*/
    }
}

