using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class RandomMethod : IPredictionMethod
    {
        public double calculateSimilarity(User u1, User u2, List<string> intersectList = null)
        {
            //this calss is not supported for random method
            return -1;
        }
        
        public PredictionMethod GetPredictionMethod()
        {
            return PredictionMethod.Random;
        }
    }
}
