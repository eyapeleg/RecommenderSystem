using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    class PredictionEngine
    {
        //TODO - check logic
        public double predictRating(User thisUser, string itemId, IList<KeyValuePair<User, double>> similarUsers)
        {
            //no similar users to this user
            if (similarUsers.Count == 0)
            {
                return 0;
            }

            double sum = 0.0;
            double rating;
            foreach (KeyValuePair<User, double> thatUserSimilarity in similarUsers)
            {
                rating = thatUserSimilarity.Key.GetRating(itemId);
                sum += rating * thatUserSimilarity.Value;
            }

            return  thisUser.GetAverageRatings() + (sum / similarUsers.Count);
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
