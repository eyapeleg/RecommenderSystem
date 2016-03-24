using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class CosineMethod 
    {
        public double calculateSimilarity(User u1, User u2)
        {
            double dotProduct = 0.0;
      
            //Get the common rated items and calcuate the dotProduct for them
            var intersectList = u1.getRatedItems().Intersect(u2.getRatedItems());
            foreach (var item in intersectList)
            {
                double u1_rating = u1.getRating(item);
                double u2_rating = u2.getRating(item);
                dotProduct += u1_rating * u2_rating;
            }

            double cos = dotProduct / (Math.Sqrt(u1.getSquaredSum()) * (Math.Sqrt(u2.getSquaredSum())));
            return cos;
        }
      
    }
}
