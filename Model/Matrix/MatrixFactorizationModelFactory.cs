using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    class MatrixFactorizationModelFactory
    {
        public MatrixFactorizationModelFactory() { }

        public MatrixFactorizationModel newRandomModel(Users users, Items items, int K, double miu)
        {
            RandomGenerator randomGenerator = new RandomGenerator();
            Dictionary<User, double> buVector = randomGenerator.newRandomVector<Users,User>(users, 0.1, -0.05);
            Dictionary<Item, double> biVector = randomGenerator.newRandomVector<Items, Item>(items, 0.1, -0.05);
            Matrix<User> p = randomGenerator.newRandomMatrix<Users,User>(users, 0.1, -0.05, K);
            Matrix<Item> q = randomGenerator.newRandomMatrix<Items, Item>(items, 0.1, -0.05, K);
            MatrixFactorizationModel model = new MatrixFactorizationModel(miu,buVector,biVector,p,q);
            return model;
        }

    }
}
