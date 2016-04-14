using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class MatrixFactorizationModel : IEnumerable<Tuple<User,Item>>
    {
        private Matrix<User> p;
        private Matrix<Item> q;

        private double miu;
        private Dictionary<User, double> buVector;
        private Dictionary<Item, double> biVector;

        public MatrixFactorizationModel(double miu, 
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

        public double getPrediction(User user, Item item)
        {
            return miu +
                   getBu(user) +
                   getBi(item) +
                   getPQmultiplication(user, item);
        }

        public double getBu(User user)
        {
            if (!buVector.ContainsKey(user))
                throw new NullReferenceException("User [" + user.GetId() + "] not found in the DB!");

            return buVector[user];
        }

        public double getBi(Item item)
        {
            if (!biVector.ContainsKey(item))
                throw new NullReferenceException("Item [" + item.GetId() + "] not found in the DB!");

            return biVector[item];
        }

        public void setBu(User user, double value)
        {
            if (!buVector.ContainsKey(user))
                throw new NullReferenceException("User [" + user.GetId() + "] not found in the DB!");

            buVector[user] = value;
        }

        public void setBi(Item item, double value)
        {
            if (!biVector.ContainsKey(item))
                throw new NullReferenceException("Item [" + item.GetId() + "] not found in the DB!");

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

        private double getPQmultiplication(User user, Item item)
        {
            List<double> userValues = p.getElementAt(user);
            List<double> itemValues = q.getElementAt(item);
            double sum = 0;

            //TODO - force length equality
            for (int i = 0; i < userValues.Count; i++)
            {
                sum += userValues.ElementAt(i) * itemValues.ElementAt(i);
            }

            return sum;
        }

        public IEnumerator<Tuple<User, Item>> GetEnumerator()
        {
            foreach (KeyValuePair<User,double> user in buVector)
            {
                foreach (KeyValuePair<Item, double> item in biVector)
                {
                    yield return Tuple.Create(user.Key,item.Key);
                
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
