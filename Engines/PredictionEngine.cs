using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
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

            return (sum/similarUsers.Count);
        }

        public double PredictRating(User user)
        {
            var dist = user.GetRatingDistribution();
            double rand = new Random().NextDouble();

            foreach (var rating in dist)
            {
                if (rand < rating.Value)
                {
                    return rating.Key;
                }
            }

            return 0;
        }
    }
}
