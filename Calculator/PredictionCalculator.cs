using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    class PredictionCalculator
    {
        //TODO - check logic
        public double calculatePrediction(User thisUser, string itemId, Dictionary<double, User> similarUsers)
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
