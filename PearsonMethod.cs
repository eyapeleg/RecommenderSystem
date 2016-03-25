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
            double denumeratorU1squareSum = 0.0;
            double denumeratorU2squareSum = 0.0;

            double u1_avg = u1.getAverageRatings();
            double u2_avg = u2.getAverageRatings();

            //TODO - foreach user, get only similar users with at least one common rated item
            var intersectList = u1.getRatedItems().Intersect(u2.getRatedItems());
            foreach (var item in intersectList)
            {
                double u1_rating = u1.getRating(item);
                double u2_rating = u2.getRating(item);
                double u1_delta = (u1_rating - u1_avg);
                double u2_delta = (u2_rating - u2_avg);
                numeratorSum += u1_delta * u2_delta;
                denumeratorU1squareSum += Math.Pow(u1_delta, 2);
                denumeratorU2squareSum += Math.Pow(u2_delta, 2);
            }
            if (intersectList.Count() == 0)
                return 0;

            return numeratorSum / (denumeratorU1squareSum * denumeratorU2squareSum);
        }
    }
}

