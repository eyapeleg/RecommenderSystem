﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class JaccardMethod : ISimilarityMethod
    {
        public double calculateSimilarity(User u1, User u2, List<string> intersectList)
        {
            double intersectCount = intersectList.Count();

            if (intersectCount == 0)
            {
                return 0;
            }

            double u1Count = u1.GetRatedItems().Count();
            double u2Count = u2.GetRatedItems().Count();

            return intersectCount / (u1Count + u2Count);
        }

        public RecommenderSystem.PredictionMethod GetPredictionMethod()
        {
            return RecommenderSystem.PredictionMethod.Jaccard;
        }
    }
}
