using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class UsersSimilarity : BoundedSortedCollection<ValueComparablePair<User, double>>
    {
        public UsersSimilarity(int maxSize) : base(maxSize) { }

        public List<KeyValuePair<User, double>> AsList()
        {
            List<ValueComparablePair<User, double>>  list = base.AsList();
                return list.ConvertAll<KeyValuePair<User,double>>(x => new KeyValuePair<User,double>(x.k,x.v));
        }

        public void add(User user, double similarity){
            base.add(new ValueComparablePair<User,double>(user, similarity));
        }
    }
}
