using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    class MatrixFactorizationEngine
    {
        private const double yRate = 0.05; //TODO - rename
        private const double lambdaRate = 0.05; //TODO - rename
        private const double minErrorThreshold = 1.0; //TODO - evalute the appropriate error threshold 
        private Users users;
        private Items items;

        public MatrixFactorizationEngine(Users users, Items items)
        {
            this.users = users;
            this.items = items;
        }

        public MatrixFactorizationModel train(int k, double miu)
        {
            MatrixFactorizationModelFactory factory = new MatrixFactorizationModelFactory();
            MatrixFactorizationModel model = factory.newRandomModel(users, items, k, miu);

            double error;
            double actualRating;
            double predictedRating;
            double prevRmse = Double.MaxValue;

            EvaluationEngine evaluationEngine = new EvaluationEngine(users);
            double currRmse = evaluationEngine.computeRMSE(model); //TODO - modify the rmse to be on the validation set

            while (prevRmse > currRmse) //TODO - add num iteration and improvment threshold conditions
            {
                foreach (Tuple<User, Item> userItemTuple in model)
                {
                    User user = userItemTuple.Item1;
                    Item item = userItemTuple.Item2;

                    actualRating = user.GetRating(item.GetId());
                    predictedRating = model.getPrediction(user, item);
                    error = actualRating - predictedRating;

                    model.setBu(user, model.getBu(user) + yRate * (error - lambdaRate * model.getBu(user)));
                    model.setBi(item, model.getBi(item) + yRate * (error - lambdaRate * model.getBi(item)));
                    //TODO -  add set to pu qi
                }

                prevRmse = currRmse;
                currRmse = evaluationEngine.computeRMSE(model); //TODO - modify the rmse to be on the validation set
            }

            return model;
       }

    }
}
