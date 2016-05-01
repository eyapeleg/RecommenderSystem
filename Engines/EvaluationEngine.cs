using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class EvaluationEngine
    {

        public EvaluationEngine()
        {
        
        }

        public List<KeyValuePair<User, string>> createTestSet(int cTrials, Users users)
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

        public Dictionary<RecommenderSystem.PredictionMethod, double> ComputeMAE(PredictionEngine predictionEngine, List<RecommenderSystem.PredictionMethod> lMethods, List<KeyValuePair<User, string>> userItemTestSet)
        {
            Dictionary<RecommenderSystem.PredictionMethod, int> countDictionary = new Dictionary<RecommenderSystem.PredictionMethod, int>();
            Dictionary<RecommenderSystem.PredictionMethod, double> maeResult = new Dictionary<RecommenderSystem.PredictionMethod, double>();

            foreach (var testedObject in userItemTestSet)
            {
                Item item = new Item(testedObject.Value);
                User user = testedObject.Key;
                
                double actual = user.GetRating(testedObject.Value);
                foreach (var lMethod in lMethods)
                {
                    double predicted = predictionEngine.Predict(lMethod, user, item);
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

        public double computeRMSE(Users validationUsers, Items validationItems, IPredictionModel model)
        {

            double sse = 0;
            double actualRating;
            double predictedRating;
            int n = 0;

            foreach (User user in validationUsers)
            {
                foreach (string itemId in user.GetRatedItems())
                {

                    Item item = validationItems.GetItemById(itemId);

                    actualRating = user.GetRating(itemId); //TODO - take only items that user rated
                    predictedRating = model.Predict(user, item);

                    sse += Math.Pow(actualRating - predictedRating, 2);
                    n++;
                }
            }
            return Math.Sqrt(sse / n);
        }

        public Tuple<double,double> computeConfidence(Users validationUsers, Items validationItems, IPredictionModel modelA, IPredictionModel modelB)
        {

            double aCounter = 0;
            double bCounter = 0;
            double aPrediction;
            double bPrediction;
            double aError;
            double bError;
            double actualRating;

            // calcualte number wins for each model
            foreach (User user in validationUsers)
            {
                foreach (string itemId in user.GetRatedItems())
                {

                    Item item = validationItems.GetItemById(itemId);                    
                    actualRating = user.GetRating(itemId);  //TODO - take only items that user rated

                    aPrediction = modelA.Predict(user, item);
                    bPrediction = modelB.Predict(user, item);

                    aError = Math.Abs(actualRating - aPrediction);
                    bError = Math.Abs(actualRating - bPrediction);

                    if (aError < bError)
                        aCounter++;
                    else if (aError > bError)
                        bCounter++;
                    else
                    {
                        aCounter += 0.5;
                        bCounter += 0.5;
                    }
                }
            }

            int n = (int)(aCounter + bCounter);
            
            // calcualte pA
            double sum=0;

            for (int i = (int)aCounter; i < n; i++)
            {
                sum += MathUtils.Factorial(n) / (MathUtils.Factorial(n - i) * MathUtils.Factorial(i));
            }

            double pA = (1 - Math.Pow(0.5, n) * sum);           

            // calculate pB
            sum = 0;
            for (int i = (int)bCounter; i < n; i++)
            {
                sum += MathUtils.Factorial(n) / (MathUtils.Factorial(n - i) * MathUtils.Factorial(i));
            }

            double pB = (1 - Math.Pow(0.5, n) * sum);

            return Tuple.Create(pA, pB);
        }
    }
}
