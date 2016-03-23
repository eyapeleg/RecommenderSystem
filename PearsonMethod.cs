using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class PearsonMethod : IPredictionMethod
    {
        Dictionary<string, double> averageRating;

        public PearsonMethod()
        {
            averageRating = new Dictionary<string, double>();
        }

        internal void calcAverageRatingPerUser(Dictionary<string, Dictionary<string, double>> userData)
        {
            Console.WriteLine("Calculate average rating per user...");

            foreach (var user in userData)
            {
                string userId = user.Key;
                Dictionary<string, double> items = user.Value;

                double avg = items.Values.Average();
                averageRating.Add(userId, avg);
            }
            Console.WriteLine("Average rating calculation completed");
        }
    }
}
