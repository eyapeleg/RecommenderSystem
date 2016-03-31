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
            double numerator = 0.0;
            double rating;
            double avgRating;

            //no similar users to this user
            if (similarUsers.Count == 0)
            {
                return thisUser.GetRandomRate();
            }

            double denominator = similarUsers.Sum(k => k.Value);

            foreach (KeyValuePair<User, double> thatUserSimilarity in similarUsers)
            {
                double w = thatUserSimilarity.Value;
                //TODO check with guy the expected behivor
                rating = thatUserSimilarity.Key.GetRating(itemId);
                avgRating = thatUserSimilarity.Key.GetAverageRatings();


                numerator += w * (rating - avgRating);
            }



            return thisUser.GetAverageRatings() + (numerator / denominator);
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
