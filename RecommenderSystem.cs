﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RecommenderSystem
{
    public class RecommenderSystem
    {
        public enum PredictionMethod { Pearson, Cosine, Random, BaseModel, Stereotypes };
        PredictionMethodsMapping predictionMethodsMapping;
        private Users users;
        private Items items;

        private SimilarityEngine similarityEngine;
        private DataLoaderEngine dataLoaderEngine;
        private PredictionEngine predictionEngine;
        private EvaluationEngine evaluationEngine;

        private ILogger logger;

        private int MAX_SIMILAR_USERS;
        private int NUM_OF_TRIALS;

        public RecommenderSystem()
        {
            logger = new InfoLogger();
            dataLoaderEngine = new DataLoaderEngine(logger);

            MAX_SIMILAR_USERS = 30;
            NUM_OF_TRIALS = 1000;
            predictionMethodsMapping = new PredictionMethodsMapping();           
        }

        public void Load(string sFileName)
        {
            Tuple<Users, Items> data = dataLoaderEngine.Load(sFileName);
            users = data.Item1;
            items = data.Item2;
            similarityEngine = new SimilarityEngine(users, MAX_SIMILAR_USERS, logger);
            predictionEngine = new PredictionEngine(users, items, predictionMethodsMapping, similarityEngine);
            evaluationEngine = new EvaluationEngine(predictionEngine, users);
        }

        public void Load(string sFileName, double dTrainSetSize)
        {
            Dictionary<string, Tuple<Users, Items>> data = dataLoaderEngine.Load(sFileName, dTrainSetSize);
            Tuple<Users, Items> trainData = data["train"];
            Tuple<Users, Items> testData = data["test"];

            //TODO: Remove before submission - validate the size of the test set
            //int count = 0;
            //for (int i = 0; i < testData.Item1.Count(); i++)
            //{
            //    var items = testData.Item1.ElementAt(i).GetRatedItems();
            //    for (int j = 0; j < items.Count; j++)
            //    {
            //        count++;
            //    }
            //}
        }

        public void TrainBaseModel(int cFeatures)
        {
            throw new NotImplementedException();
        }
        public void TrainStereotypes(int cStereotypes)
        {
            throw new NotImplementedException();
        }

        //return a list of the ids of all the users in the dataset
        public List<string> GetAllUsers()
        {
            return users.GetAllUsers();
        }

        //returns a list of all the items in the dataset
        public List<string> GetAllItems()
        {
            return items.GetAllItems();
        }

        //returns the list of all items that the given user has rated in the dataset
        public List<string> GetRatedItems(string sUID)
        {
            return users.GetRatedItems(sUID);
        }

        //Returns a user-item rating that appears in the dataset (not predicted)
        public double GetRating(string sUID, string sIID)
        {
            return users.GetRating(sUID, sIID);
        }

        public Dictionary<double, int> GetRatingsHistogram(string sUID)
        {
            throw new NotImplementedException();
        }

        //predict a rating for a user item pair using the specified method
        public double PredictRating(PredictionMethod m, string sUID, string sIID)
        {
            User user = users.getUserById(sUID);
            return predictionEngine.PredictRating(m, user, sIID);
        }

        //Compute MAE (mean absolute error) for a set of rating prediction methods over the same user-item pairs
        //cTrials specifies the number of user-item pairs to be tested
        public Dictionary<PredictionMethod, double> ComputeMAE(List<PredictionMethod> lMethods, int cTrials)
        {
            List<KeyValuePair<User, string>> userItemTestSet = evaluationEngine.createTestSet(cTrials);
            return evaluationEngine.ComputeMAE(lMethods, userItemTestSet);
        }

        public Dictionary<PredictionMethod, double> ComputeRMSE(List<PredictionMethod> lMethods, int cTrials)
        {
            throw new NotImplementedException();
        }

        public Dictionary<PredictionMethod, double> ComputeRMSE(List<PredictionMethod> lMethods, out Dictionary<PredictionMethod, Dictionary<PredictionMethod, double>> dConfidence)
        {
            throw new NotImplementedException();
        }
    }
}
