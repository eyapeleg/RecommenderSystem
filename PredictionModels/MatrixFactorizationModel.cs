using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class MatrixFactorizationModel : IPredictionModel
    {
        private static readonly double yRate = Double.Parse(ConfigurationManager.AppSettings["yRate"]);
        private static readonly double lambdaRate = Double.Parse(ConfigurationManager.AppSettings["lambdaRate"]);
        private const double minErrorThreshold = 1.0; //TODO - evalute the appropriate error threshold 
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

            EvaluationEngine evaluationEngine = new EvaluationEngine();
            double currRmse = evaluationEngine.computeRMSE(validationUsers, validationItems, this);
            int iteration = 1; //TODO - remove that before submission

            while (prevRmse > currRmse) //TODO - add num iteration and improvment threshold conditions
            {
                Console.WriteLine("Iteraion: {0}, Current RMSE: {1}", iteration++, currRmse);
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
            }

            Console.WriteLine(String.Format("Final Result - RMSE = {0}", prevRmse));
       }

        public double Predict(User user, Item item)
        {
            return mf.Predict(user, item);
        }
    }
}
