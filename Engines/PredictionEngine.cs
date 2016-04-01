using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class PredictionEngine
    {
        private Users users;
        private Items items;
        private PredictionMethodsMapping predictionMethodsMapping;
        private SimilarityEngine similarityEngine;

        public PredictionEngine(Users users, Items items, PredictionMethodsMapping predictionMethodsMapping, SimilarityEngine similarityEngine)
        {
            this.users = users;
            this.items = items;
            this.predictionMethodsMapping = predictionMethodsMapping;
            this.similarityEngine = similarityEngine;
        }
        
        private double calculateRating(User thisUser, string itemId, IList<KeyValuePair<User, double>> similarUsers)
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


        public double PredictRating(RecommenderSystem.PredictionMethod m, User user, string sIID)
        {

            var candidateUsers = items.GetItemById(sIID).Keys.ToList();

            //in case the current user is the only one that predict this item return the random rating or otherwise if the current user has only one rated item
            if ((candidateUsers.Count == 1 && candidateUsers.Contains(user.GetId())) || user.GetRatedItems().Count < 2)
            {
                return user.GetRandomRate();
            }

            IPredictionMethod predictionMethod = predictionMethodsMapping.getPredictionMethod(m);

            if (m != RecommenderSystem.PredictionMethod.Random)
            {
                IList<KeyValuePair<User, double>> similarUsers = similarityEngine.calculateSimilarity(predictionMethod, user, candidateUsers);
                double prediction = calculateRating(user, sIID, similarUsers);
                return prediction > 10 ? 10 : prediction;
            }

            return user.GetRandomRate();
        }

    }
}
