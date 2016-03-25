using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class RandomMethod 
    {
        public double PredictRating(User u)
        {
            var dist = u.GetRatingDistribution();
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
