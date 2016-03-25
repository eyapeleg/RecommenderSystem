using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    class UserSimilarities
    {
        Dictionary<PredictionMethod, BoundedSortedDictionary<double,User>> userSimilarities;

        public UserSimilarities()
        {
            userSimilarities = new Dictionary<PredictionMethod, BoundedSortedDictionary<double, User>>();
        }

        public void add(PredictionMethod method, User user, double similarity)
        {
            if (!userSimilarities.ContainsKey(method))
                throw new NotSupportedException("Prediction method does not exist!");

            userSimilarities[method].add(similarity, user);
        }

        public IEnumerable<KeyValuePair<double, User>> getSimilarUsers(PredictionMethod method)
        {
            if (!userSimilarities.ContainsKey(method))
                throw new NotSupportedException("Prediction method does not exist!");

            return userSimilarities[method].getEnumerable();
        }
    }
}
