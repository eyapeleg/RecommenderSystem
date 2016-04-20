using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class RandomMethod : ISimilarityMethod
    {
        public double calculateSimilarity(User u1, User u2, List<string> intersectList = null)
        {
            //this method is not supported for random method
            return -1;
        }

        public RecommenderSystem.PredictionMethod GetPredictionMethod()
        {
            return RecommenderSystem.PredictionMethod.Random;
        }
    }
}
