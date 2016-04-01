﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class EvaluationEngine
    {
        private PredictionEngine predictionEngine;
        private Users users;

        public EvaluationEngine(PredictionEngine predictionEngine, Users users)
        {
            this.predictionEngine = predictionEngine;
            this.users = users;
        }

        public List<KeyValuePair<User, string>> createTestSet(int cTrials)
        {
            List<KeyValuePair<User, string>> userItemTestSet = new List<KeyValuePair<User, string>>();
            Random rnd = new Random();

            do
            {
                int userIdx = rnd.Next(users.Count());
                User testedUser = users.ElementAt(userIdx);
                List<string> ratedItems = testedUser.GetRatedItems();

                //choose only users that rated more than one item
                if (ratedItems.Count < 2)
                {
                    continue;
                }

                int itemIdx = rnd.Next(ratedItems.Count);
                string testedItemId = ratedItems.ElementAt(itemIdx);

                KeyValuePair<User, string>  userItemPair = new KeyValuePair<User, string>(testedUser, testedItemId);

                if (!userItemTestSet.Contains(userItemPair))
                {
                    userItemTestSet.Add(userItemPair);
                }
            } while (userItemTestSet.Count != cTrials);

            return userItemTestSet;
        }

        public Dictionary<RecommenderSystem.PredictionMethod, double> ComputeMAE(List<RecommenderSystem.PredictionMethod> lMethods, List<KeyValuePair<User, string>> userItemTestSet)
        {
            Dictionary<RecommenderSystem.PredictionMethod, int> countDictionary = new Dictionary<RecommenderSystem.PredictionMethod, int>();
            Dictionary<RecommenderSystem.PredictionMethod, double> maeResult = new Dictionary<RecommenderSystem.PredictionMethod, double>();

            foreach (var testedObject in userItemTestSet)
            {
                double actual = testedObject.Key.GetRating(testedObject.Value);
                foreach (var lMethod in lMethods)
                {
                    double predicted = predictionEngine.PredictRating(lMethod, testedObject.Key, testedObject.Value);
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