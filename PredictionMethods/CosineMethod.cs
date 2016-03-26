using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class CosineMethod : IPredictionMethod
    {
        public double calculateSimilarity(User u1, User u2)
        {
            double dotProduct = 0.0;

            //Get the common rated items and calcuate dotProduct for them
            var intersectList = u1.GetRatedItems().Intersect(u2.GetRatedItems());
            var list = intersectList as string[] ?? intersectList.ToArray();

            foreach (var item in list)
            {
                double u1Rating = u1.GetRating(item);
                double u2Rating = u2.GetRating(item);
                dotProduct += u1Rating*u2Rating;
            }
            if (!list.Any())
                return 0;

            double cos = dotProduct/(Math.Sqrt(u1.GetSquaredSum())*(Math.Sqrt(u2.GetSquaredSum())));
            return cos;
        }

        public PredictionMethod GetPredictionMethod()
        {
            return PredictionMethod.Cosine;
        }
    }
}
