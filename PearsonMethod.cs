using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class PearsonMethod : IPredictionMethod
    {
        private Dictionary<string, double> averageRating;
        private RecommenderSystem recommenderSystem;

        public PearsonMethod(RecommenderSystem recommenderSystem)
        {
            // TODO: Complete member initialization
            this.recommenderSystem = recommenderSystem;
            averageRating = new Dictionary<string, double>();
        }

        internal void calcAverageRatingPerUser()
        {
            Console.WriteLine("Calculate average rating per user...");

            foreach (var user in recommenderSystem.userData)
            {
                string userId = user.Key;
                Dictionary<string, double> items = user.Value;

                double avg = items.Values.Average();
                averageRating.Add(userId, avg);
            }
            Console.WriteLine("Average rating calculation completed");
        }

        internal double PredictRating(string sUID, string sIID)
        {
            throw new NotImplementedException();
        }
    }
}
