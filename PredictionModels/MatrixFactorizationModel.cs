using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class MatrixFactorizationModel : IPredictionModel
    {
        private static readonly double yRate = 0.05; //Double.Parse(ConfigurationManager.AppSettings["yRate"]);
        private static readonly double lambdaRate = 0.05; //Double.Parse(ConfigurationManager.AppSettings["lambdaRate"]);
        private const double minErrorThreshold = 1.0; //TODO Eyal- evalute the appropriate error threshold 
        private double miu;
        private Users trainUsers;
        private Items trainItems;
        private Users validationUsers;
        private Items validationItems;
        private MatrixFactorization mf;
        private int cFeatures;

        public MatrixFactorizationModel(Users trainUsers, Items trainItems, Users validationUsers, Items validationItems, int cFeatures, double miu)
        {
            this.trainUsers = trainUsers;
            this.trainItems = trainItems;
            this.validationUsers = validationUsers;
            this.validationItems = validationItems;
            this.cFeatures = cFeatures;
            this.miu = miu;
        }

        public void Train()
        {
            Console.WriteLine("***************** Train Matrix Factorization Model *********************");
            mf = MatrixFactorization.newRandomMatrix(trainUsers, trainItems, cFeatures, miu);

            double error;
            double actualRating;
            double predictedRating;
            double prevRmse = Double.MaxValue;

            EvaluationEngine evaluationEngine = new EvaluationEngine(miu);
            double currRmse = evaluationEngine.computeRMSE(validationUsers, validationItems, this);
            int iteration = 1;

            while (prevRmse > currRmse) //TODO Eyal - add num iteration and improvment threshold conditions
            {
                Console.WriteLine("Iteraion: {0}, Current RMSE: {1}", iteration, currRmse);
                foreach(User user in trainUsers)
                {
                    foreach (string itemId in user.GetRatedItems())
                    {

                        Item item = trainItems.GetItemById(itemId);

                        actualRating = user.GetRating(item.GetId());
                        predictedRating = mf.Predict(user, item);
                        error = actualRating - predictedRating;

                        var userBias = mf.getBu(user);
                        mf.setBu(user, userBias + yRate * (error - lambdaRate * userBias));

                        var itemBias = mf.getBi(item);
                        mf.setBi(item, itemBias + yRate * (error - lambdaRate * itemBias));

                        //update Qi and Pu
                        mf.UpdateQiAndPu(item, user, error);
                    }
                }

                prevRmse = currRmse;
                currRmse = evaluationEngine.computeRMSE(validationUsers, validationItems,this);
                iteration++;
            }

            Console.WriteLine(String.Format("Final Result - RMSE = {0}", prevRmse));
       }

        public double Predict(User user, Item item)
        {
            return mf.Predict(user, item);
        }

        public double calculateSimilarity(User u1, User u2)
        {
            var pu1 = mf.getPu(u1);
            var pu2 = mf.getPu(u2);

            double similarityVal = pu1.Select((t, i) => Math.Pow(t - pu2[i], 2)).Sum();

            return Math.Sqrt(similarityVal);
        }
    }
}
