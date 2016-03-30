using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class PearsonMethod : IPredictionMethod
    {
        public double calculateSimilarity(User u1, User u2, List<string> intersectList)
        {
            //if (intersectList.Count < 5)
            //    return 0.0;

            double numeratorSum = 0.0;
            double denumeratorU1SumSquare = 0.0;
            double denumeratorU2SumSquare = 0.0;

            double u1Avg = u1.GetAverageRatings();
            double u2Avg = u2.GetAverageRatings();

            foreach (var item in intersectList)
            {
                double u1_rating = u1.GetRating(item);
                double u2_rating = u2.GetRating(item);
                double u1_delta = (u1_rating - u1Avg);
                double u2_delta = (u2_rating - u2Avg);
                numeratorSum += u1_delta * u2_delta;
                denumeratorU1SumSquare += Math.Pow(u1_delta, 2);
                denumeratorU2SumSquare += Math.Pow(u2_delta, 2);
            }
            if (denumeratorU1SumSquare == 0 || denumeratorU2SumSquare == 0)
                return 0.0;

            return numeratorSum / (Math.Sqrt(denumeratorU1SumSquare) * Math.Sqrt(denumeratorU2SumSquare));
        }

        public PredictionMethod GetPredictionMethod()
        {
            return PredictionMethod.Pearson;
        }
    }
}

