using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class BaseModelMethod : ISimilarityMethod
    {
        private MatrixFactorizationModel model;
        public BaseModelMethod(IPredictionModel model)
        {
            this.model = (MatrixFactorizationModel)model;
        }
        public double calculateSimilarity(User u1, User u2, List<string> intersectList)
        {
            return 1 / (1 + model.calculateSimilarity(u1, u2));
        }

        public RecommenderSystem.PredictionMethod GetPredictionMethod()
        {
            return RecommenderSystem.PredictionMethod.BaseModel;
        }
    }
}
