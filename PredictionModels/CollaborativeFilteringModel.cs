using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class CollaborativeFilteringModel : IPredictionModel
    {
        private SimilarityEngine similarityEngine;
        private Users users;
        private Items items;
        private ISimilarityMethod predictionMethod;

        public CollaborativeFilteringModel(Users users, Items items, SimilarityEngine similarityEngine, ISimilarityMethod predictionMethod)
        {
            this.similarityEngine = similarityEngine;
            this.users = users;
            this.items = items;
            this.predictionMethod = predictionMethod;
        }

        public void Train()
        {
            return;
        }

        public double Predict(User user, Item item)
        {
            var candidateUsers = item.GetRatingUsers();

            //in case the current user is the only one that predict this item return the random rating or otherwise if the current user has only one rated item
            if ((candidateUsers.Count == 1 && candidateUsers.Contains(user.GetId())) || user.GetRatedItems().Count < 2)
            {
                return user.GetRandomRate();
            }

            var similarUsers = similarityEngine.calculateSimilarity(predictionMethod, user, candidateUsers);

            //no similar users to this user
            if (similarUsers.Count == 0)
            {
                return user.GetRandomRate();
            }

            return CalculateRating(user, item, similarUsers);
        }

        #region private methods
        private double CalculateRating(User user, Item item, IList<KeyValuePair<User, double>> similarUsers)
        {
            double numerator = 0.0;
            double rating;
            double avgRating;

            double denominator = similarUsers.Sum(k => k.Value);

            foreach (KeyValuePair<User, double> thatUserSimilarity in similarUsers)
            {
                double w = thatUserSimilarity.Value;
                rating = thatUserSimilarity.Key.GetRating(item.GetId());
                avgRating = thatUserSimilarity.Key.GetAverageRatings();


                numerator += w * (rating - avgRating);
            }

            return user.GetAverageRatings() + (numerator / denominator);
        }

        #endregion

    }
}
