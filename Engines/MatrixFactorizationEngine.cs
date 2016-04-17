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
        private Users validationUsers;
        private Items validationItems;

        public MatrixFactorizationEngine(Users trainUsers, Items trainItems, Users validationUsers, Items validationItems)
        {
            this.trainUsers = trainUsers;
            this.trainItems = trainItems;
            this.validationUsers = validationUsers;
            this.validationItems = validationItems;
        }

        public MatrixFactorizationModel train(int k, double miu)
        {
            MatrixFactorizationModelFactory factory = new MatrixFactorizationModelFactory();
            MatrixFactorizationModel model = factory.newRandomModel(trainUsers, trainItems, k, miu);

            double error;
            double actualRating;
            double predictedRating;
            double prevRmse = Double.MaxValue;

            EvaluationEngine evaluationEngine = new EvaluationEngine();
            double currRmse = evaluationEngine.computeRMSE(validationUsers, validationItems, model); 

            while (prevRmse > currRmse) //TODO - add num iteration and improvment threshold conditions
            {
                foreach(User user in trainUsers)
                {
                    foreach (string itemId in user.GetRatedItems())
                    {

                        Item item = trainItems.GetItemById(itemId);

                        actualRating = user.GetRating(item.GetId());
                        predictedRating = model.getPrediction(user, item);
                        error = actualRating - predictedRating;

                        model.setBu(user, model.getBu(user) + yRate * (error - lambdaRate * model.getBu(user)));
                        model.setBi(item, model.getBi(item) + yRate * (error - lambdaRate * model.getBi(item)));

                        //update Qi and Pu
                        var pu = model.getPu(user);
                        var qi = model.getQi(item);

                        var multipleErrorPu = MathUtils.MultipleScalarByVector(error, pu);
                        var multipleLambdaQi = MathUtils.MultipleScalarByVector(lambdaRate, qi);
                        var minusQi = MathUtils.MinusVectors(multipleErrorPu, multipleLambdaQi);
                        var multipleYRateQi = MathUtils.MultipleScalarByVector(yRate, minusQi);
                        var newQi = MathUtils.AdditionVectors(qi, multipleYRateQi);
                        model.setQi(item, newQi);

                        var multipleErrorQi = MathUtils.MultipleScalarByVector(error, qi);
                        var multipleLambdaPu = MathUtils.MultipleScalarByVector(lambdaRate, pu);
                        var minusPu = MathUtils.MinusVectors(multipleErrorQi, multipleLambdaPu);
                        var multipleYRatePu = MathUtils.MultipleScalarByVector(yRate, minusPu);
                        var newPu = MathUtils.AdditionVectors(pu, multipleYRatePu);
                        model.setPu(user, newPu);
                    }
                }

                prevRmse = currRmse;
                currRmse = evaluationEngine.computeRMSE(validationUsers, validationItems,model);
            }

            return model;
       }

    }
}
