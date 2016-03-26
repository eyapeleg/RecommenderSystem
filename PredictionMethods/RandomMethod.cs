using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class RandomMethod:IPredictionMethod
    {
        public double calculateSimilarity(User u1, User u2)
        {
            var dist = u1.GetRatingDistribution();
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
        
        public PredictionMethod GetPredictionMethod()
        {
            return PredictionMethod.Random;
        }
    }
}
