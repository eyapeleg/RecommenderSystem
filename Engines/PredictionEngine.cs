using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class PredictionEngine
    {
        private Dictionary<RecommenderSystem.PredictionMethod, IPredictionModel> predictionMethodsDictionary;

        public PredictionEngine()
        {
            predictionMethodsDictionary = new Dictionary<RecommenderSystem.PredictionMethod, IPredictionModel>();
        }

        public void Train(RecommenderSystem.PredictionMethod method)
        {
            predictionMethodsDictionary[method].Train();
        }

        public double Predict(RecommenderSystem.PredictionMethod method, User user, Item item)
        {
            return predictionMethodsDictionary[method].Predict(user, item);
        }

        public void addModel(RecommenderSystem.PredictionMethod method, IPredictionModel model)
        {
            if (!predictionMethodsDictionary.ContainsKey(method))
            {
                predictionMethodsDictionary.Add(method, model);
            }      
        }

    }
}
