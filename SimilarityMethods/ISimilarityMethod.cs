using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public interface ISimilarityMethod
    {
        double calculateSimilarity(User u1, User u2, List<string> intersectList );

        RecommenderSystem.PredictionMethod GetPredictionMethod();
    }
}
