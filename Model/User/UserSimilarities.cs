using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1
{
    public class UserSimilarities
    {
        Dictionary<PredictionMethod, BoundedSortedDictionary<double,User>> userSimilarities;

        public UserSimilarities(ISet<PredictionMethod> predictionMethods)
        {
            userSimilarities = new Dictionary<PredictionMethod, BoundedSortedDictionary<double, User>>();
            foreach (PredictionMethod predictionMethod in predictionMethods)
            {
                //TODO Change the size of the similarity list to be generic
                userSimilarities.Add(predictionMethod, new BoundedSortedDictionary<double, User>(20));
            }
            
        }

        public void Add(PredictionMethod method, User user, double similarity)
        {
              userSimilarities[method].add(similarity, user);
        }

        public IEnumerable<KeyValuePair<double, User>> GetSimilarUsers(PredictionMethod method)
        {
            if (!userSimilarities.ContainsKey(method))
                throw new NotSupportedException("Prediction method does not exist!");

            return (IEnumerable<KeyValuePair<double, User>>) userSimilarities[method].AsEnumerable().GetEnumerator();
        }
    }
}
