using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    class PredictionEngine
    {
        //TODO - check logic
        public double predictRating(User thisUser, string itemId, IList<KeyValuePair<double, User>> similarUsers)
        {
            double sum = 0.0;
            double rating;
            foreach (KeyValuePair<double,User> thatUserSimilarity in similarUsers)
            {
                rating = thatUserSimilarity.Value.GetRating(itemId);
                sum += rating * thatUserSimilarity.Key;
            }

            return (sum/(double)similarUsers.Count());
        }
    }
}
