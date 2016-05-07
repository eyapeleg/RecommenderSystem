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
            if(item == null && user != null) //in case the item doesn't exist take the random rate for that user
            {
                return user.GetRandomRate();
            }

            if(user == null && item != null) //in case the item exist but the user not, take the average rating of that item
            {
                return item.GetAverageRatings();
            }

            if(user == null && item == null) //otherwise, both user and item are not exist, take the average rating of all system
            {
                return -1;
            }

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

                if (rating == 0) //in case the similar user didn't rate the item, take the random user's rating 
                {
                    rating = thatUserSimilarity.Key.GetRandomRate();
                }
                avgRating = thatUserSimilarity.Key.GetAverageRatings();
                numerator += w * (rating - avgRating);
            }

            return user.GetAverageRatings() + (numerator / denominator);
        }

        #endregion

    }
}
