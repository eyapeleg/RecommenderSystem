using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class PredictionMethodsMapping
    {
        Dictionary<RecommenderSystem.PredictionMethod, IPredictionMethod> predictionMethodsDictionary;

        public PredictionMethodsMapping()
        {
            predictionMethodsDictionary = new Dictionary<RecommenderSystem.PredictionMethod, IPredictionMethod>(){
                {RecommenderSystem.PredictionMethod.Pearson,new PearsonMethod()},
                {RecommenderSystem.PredictionMethod.Cosine, new CosineMethod()}, 
                {RecommenderSystem.PredictionMethod.Random, new RandomMethod()}};
        }

        public IPredictionMethod getPredictionMethod(RecommenderSystem.PredictionMethod predictionMethod)
        {
            if (!predictionMethodsDictionary.ContainsKey(predictionMethod))
                throw new ArgumentException(string.Format("Method " + "[{0}]" + " does not exist!", predictionMethod));

            return predictionMethodsDictionary[predictionMethod];
        }
    }
}
