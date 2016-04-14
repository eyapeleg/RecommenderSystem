using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    class MatrixFactorizationEngine
    {
        private const double learningRate = 0.05;
        private const double lambdaRate = 0.05; //TODO - rename

        public MatrixFactorizationEngine()
        {
        }

        public MatrixFactorizationModel train(Users users, Items items, int k, double miu)
        {
            MatrixFactorizationModelFactory factory = new MatrixFactorizationModelFactory();
            MatrixFactorizationModel model = factory.newRandomModel(users, items, k, miu);

            throw new NotImplementedException();
        }
    }
}
