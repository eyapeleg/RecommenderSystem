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
        Dictionary<PredictionMethod, IPredictionMethod> predictionMethodsDictionary;
        public enum PredictionMethod { Pearson, Cosine, Random };
        private Users users;
        private Items items;
        private SimilarityEngine similarityEngine;
        private DataLoaderEngine dataLoaderEngine;
        private PredictionEngine predictionEngine;
        private ILogger logger;

        private int MAX_SIMILAR_USERS;

        public RecommenderSystem()
        {
            logger = new InfoLogger();
            dataLoaderEngine = new DataLoaderEngine(logger);
            predictionEngine = new PredictionEngine();

            MAX_SIMILAR_USERS = 30;
            predictionMethodsDictionary = new Dictionary<PredictionMethod, IPredictionMethod>(){
                {PredictionMethod.Pearson,new PearsonMethod()},
                {PredictionMethod.Cosine, new CosineMethod()}, //TODO - modify to the corresponding method
                {PredictionMethod.Random, new RandomMethod()} //TODO - modify to the corresponding method
            };

            
        }

        public void Load(string sFileName)
        {
            
            Tuple<Users, Items> data = dataLoaderEngine.Load(sFileName);
            users = data.Item1;
            items = data.Item2;
            similarityEngine = new SimilarityEngine(users, MAX_SIMILAR_USERS, logger);
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

        //predict a rating for a user item pair using the specified method
        public double PredictRating(PredictionMethod m, string sUID, string sIID)
        {
            
            User user = users.getUserById(sUID);
            var candidateUsers = items.GetItemById(sIID).Keys.ToList();

            //in case the current user is the only one that predict this item return the random rating or otherwise if the current user has only one rated item
            if((candidateUsers.Count == 1 && candidateUsers.Contains(user.GetId())) || user.GetRatedItems().Count < 2)
            {
                return user.GetRandomRate();
            }

            if (!predictionMethodsDictionary.ContainsKey(m))
                throw new ArgumentException(string.Format("Method " + "[{0}]" + " does not exist!", m) );

            IPredictionMethod predictionMethod = predictionMethodsDictionary[m];

            if (m != PredictionMethod.Random)
            {
                IList<KeyValuePair<User, double>> similarUsers = similarityEngine.calculateSimilarity(predictionMethod, user, candidateUsers);
                double prediction = predictionEngine.predictRating(user, sIID, similarUsers);
                return prediction > 10 ? 10 : prediction; 
            }

            return users.getUserById(sUID).GetRandomRate();
            //return predictionEngine.PredictRating(users.getUserById(sUID));
        }

        //Compute MAE (mean absolute error) for a set of rating prediction methods over the same user-item pairs
        //cTrials specifies the number of user-item pairs to be tested
        public Dictionary<PredictionMethod, double> ComputeMAE(List<PredictionMethod> lMethods, int cTrials)
        {
            List<KeyValuePair<string, string>> testedUserItem = new List<KeyValuePair<string, string>>();
            Random rnd = new Random();
            Dictionary<PredictionMethod, double> maeResult = new Dictionary<PredictionMethod, double>();
            Dictionary<PredictionMethod, int> countDictionary = new Dictionary<PredictionMethod, int>();

            //create <userId,ItemId> test set
            do
            {
                int userIdx = rnd.Next(users.Count());
                var testedUser = users.ElementAt(userIdx);
                var testedUserId = testedUser.GetId();
                var ratedItems = testedUser.GetRatedItems();

                //choose only users that rated more than one item
                if (ratedItems.Count < 2)
                {
                    continue;
                }

                int itemIdx = rnd.Next(ratedItems.Count);
                var testedItemId = ratedItems.ElementAt(itemIdx);

                var keyValuePair = new KeyValuePair<string, string>(testedUserId, testedItemId);

                if (!testedUserItem.Contains(keyValuePair))
                {
                    testedUserItem.Add(new KeyValuePair<string, string>(testedUserId, testedItemId));
                }
            } while (testedUserItem.Count != cTrials);

            foreach (var testedObject in testedUserItem)
            {
                double actual = users.getUserById(testedObject.Key).GetRating(testedObject.Value);
                foreach (var lMethod in lMethods)
                {
                    double predicted = PredictRating(lMethod, testedObject.Key, testedObject.Value);
                    double error = Math.Abs(predicted - actual);
                    
                    if (!maeResult.ContainsKey(lMethod))
                    {
                        maeResult.Add(lMethod, error);
                        countDictionary.Add(lMethod, 1);
                    }
                    else
                    {
                        maeResult[lMethod] += error;
                        countDictionary[lMethod]++;
                    }
                }
            }

            foreach (var lMethod in lMethods)
            {
                maeResult[lMethod] = maeResult[lMethod] / countDictionary[lMethod];
            }
            
            return maeResult;
        }
    }
}
