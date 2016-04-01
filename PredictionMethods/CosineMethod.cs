using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class CosineMethod : IPredictionMethod
    {
        public double calculateSimilarity(User u1, User u2, List<string> intersectList)
        {
            double dotProduct = 0.0;

            foreach (var item in intersectList)
            {
                double u1Rating = u1.GetRating(item);
                double u2Rating = u2.GetRating(item);
                dotProduct += u1Rating*u2Rating;
            }

            //todo check that condition
            if (!intersectList.Any())
                u1.GetRandomRate();

            double cos = dotProduct/(Math.Sqrt(u1.GetSquaredSum())*Math.Sqrt(u2.GetSquaredSum()));
            return cos;
        }

        public RecommenderSystem.PredictionMethod GetPredictionMethod()
        {
            return RecommenderSystem.PredictionMethod.Cosine;
        }
    }
}
