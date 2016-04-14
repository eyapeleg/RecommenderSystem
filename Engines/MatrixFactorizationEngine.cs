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
        private Users trainUsers;
        private Items trainItems;
        private Users validatationUsers;
        private Items validatationItems;

        public MatrixFactorizationEngine(Users users, Items items, int dsSize)
        {

            double trainSize = Math.Round(dsSize * 0.95);

            var data = DataUtils.DataSplit(0.95, trainSize, new Tuple<Users, Items>(users, items),
                RecommenderSystem.DatasetType.Validation, RecommenderSystem.DatasetType.Train);

            trainUsers = data[RecommenderSystem.DatasetType.Train].Item1;
            trainItems = data[RecommenderSystem.DatasetType.Train].Item2;
            validatationUsers = data[RecommenderSystem.DatasetType.Validation].Item1;
            validatationItems = data[RecommenderSystem.DatasetType.Validation].Item2;
        }

        public MatrixFactorizationModel train(int k, double miu)
        {
            MatrixFactorizationModelFactory factory = new MatrixFactorizationModelFactory();
            MatrixFactorizationModel model = factory.newRandomModel(trainUsers, trainItems, k, miu);

            double error;
            double actualRating;
            double predictedRating;
            double prevRmse = Double.MaxValue;

            EvaluationEngine evaluationEngine = new EvaluationEngine(validatationUsers);
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
