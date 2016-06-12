using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class FactorizationMatrix 
    {
        private Matrix<User> p;
        private Matrix<Item> q;
        private double miu;
        private Dictionary<User, double> buVector;
        private Dictionary<Item, double> biVector;

        private static readonly double yRate = 0.05; //Double.Parse(ConfigurationManager.AppSettings["yRate"]);
        private static readonly double lambdaRate = 0.05; //Double.Parse(ConfigurationManager.AppSettings["lambdaRate"]);

        public FactorizationMatrix(double miu, 
                                        Dictionary<User, double> buVector,
                                        Dictionary<Item, double> biVector,
                                        Matrix<User> p,
                                        Matrix<Item> q)
        {
            this.miu = miu;
            this.buVector = buVector;
            this.biVector = biVector;
            this.p = p;
            this.q = q;
        }

        public static FactorizationMatrix newRandomMatrix(Users users, Items items, int K, double miu)
        {
            RandomGenerator randomGenerator = new RandomGenerator();
            Dictionary<User, double> buVector = randomGenerator.newRandomVector<Users, User>(users, 0.1, -0.05);
            Dictionary<Item, double> biVector = randomGenerator.newRandomVector<Items, Item>(items, 0.1, -0.05);
            Matrix<User> p = randomGenerator.newRandomMatrix<Users, User>(users, 0.1, -0.05, K);
            Matrix<Item> q = randomGenerator.newRandomMatrix<Items, Item>(items, 0.1, -0.05, K);
            FactorizationMatrix model = new FactorizationMatrix(miu, buVector, biVector, p, q);
            return model;
        }

        public double Predict(User user, Item item)
        {
            try
            {
                return miu +
                       getBu(user) +
                       getBi(item) +
                       InnerProduct(user, item);
            }
            catch (UserNotFoundException e)
            {
                //TODO
                throw new NotImplementedException();
            }
            catch (ItemNotFoundException e)
            {
                //TODO
                throw new NotImplementedException();
            }
        }

        public double getBu(User user)
        {
            if (user == null || !buVector.ContainsKey(user))
                throw new UserNotFoundException("User [" + user.GetId() + "] not found in the DB!");

            return buVector[user];
        }

        public double getBi(Item item)
        {
            if (!biVector.ContainsKey(item))
                throw new ItemNotFoundException("Item [" + item.GetId() + "] not found in the DB!");

            return biVector[item];
        }

        public void setBu(User user, double value)
        {
            if (!buVector.ContainsKey(user))
                throw new UserNotFoundException("User [" + user.GetId() + "] not found in the DB!");

            buVector[user] = value;
        }

        public void setBi(Item item, double value)
        {
            if (!biVector.ContainsKey(item))
                throw new ItemNotFoundException("Item [" + item.GetId() + "] not found in the DB!");

            biVector[item] = value;
        }


        public List<double> getPu(User user)
        {
            return p.getElementAt(user);
        }

        public List<double> getQi(Item item)
        {
            return q.getElementAt(item);
        }

        private double InnerProduct(User user, Item item)
        {
            List<double> userValues = p.getElementAt(user);
            List<double> itemValues = q.getElementAt(item);

            return MathUtils.InnerProduct(userValues, itemValues);
        }

        public void setPu(User user, List<double> values)
        {
            p.setElementAt(user, values);
        }

        public void setQi(Item item, List<double> newValues)
        {
            q.setElementAt(item, newValues);
        }

        public void UpdateQiAndPu(Item item, User user, double error)
        {
            var currentQi = getQi(item);
            var currentPu = getPu(user);

            var newQi = MathUtils.AdditionVectors(currentQi, ComputeNewVectorValues(currentPu, currentQi, error));
            var newPu = MathUtils.AdditionVectors(currentPu, ComputeNewVectorValues(currentQi, currentPu, error));
            setQi(item, newQi);
            setPu(user, newPu);
        }


        #region private methods
        private List<double> ComputeNewVectorValues(List<double> multipleErrorElement, List<double> multipleLambdaRateElement, double error)
        {
            var multipleError = MathUtils.MultipleScalarByVector(error, multipleErrorElement);
            var multipleLambdaRate = MathUtils.MultipleScalarByVector(lambdaRate, multipleLambdaRateElement);
            var minusVectors = MathUtils.MinusVectors(multipleError, multipleLambdaRate);
            return MathUtils.MultipleScalarByVector(yRate, minusVectors);
        }

        #endregion
    }
}
