using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class ValueComparablePair<K, V> : IComparable<ValueComparablePair<K, V>>
            where V : IComparable<V>
            where K : IComparable<K>
        {
            public K k;
            public V v;

            public ValueComparablePair(K k, V v)
            {
                this.k = k;
                this.v = v;
            }

            public int CompareTo(ValueComparablePair<K, V> that)
            {
                int res = this.v.CompareTo(that.v);
                if (res == 0)
                    return this.k.CompareTo(that.k);
                return res;
            }

            public override bool Equals(object obj)
            {
                if (obj == null || !this.GetType().Name.Equals(obj.GetType().Name))
                    return false;

                if (obj == this)
                    return true;

                if (((ValueComparablePair<K, V>)obj).k.Equals(this.k) )
                    return true;

                return false;
            }

            public override int GetHashCode()
            {
                return k.GetHashCode() ^ v.GetHashCode();
            }

        }
}
