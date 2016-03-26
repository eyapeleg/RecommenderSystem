using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class PearsonMethod : IPredictionMethod
    {
        public double calculateSimilarity(User u1, User u2)
        {
            double numeratorSum = 0.0;
            double denumeratorU1SumSquare = 0.0;
            double denumeratorU2SumSquare = 0.0;

            double u1Avg = u1.GetAverageRatings();
            double u2Avg = u2.GetAverageRatings();

            //TODO - foreach user, get only similar users with at least one common rated item
            var intersectList = u1.GetRatedItems().Intersect(u2.GetRatedItems());
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
            if (!intersectList.Any() || denumeratorU1SumSquare == 0 || denumeratorU2SumSquare == 0)
                return 0;

            return numeratorSum / (denumeratorU1SumSquare * denumeratorU2SumSquare);
        }

        public PredictionMethod GetPredictionMethod()
        {
            return PredictionMethod.Pearson;
        }
    }
}

