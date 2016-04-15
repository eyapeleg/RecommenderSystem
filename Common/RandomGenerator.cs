using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class RandomGenerator
    {
        private Random rand;

        public RandomGenerator() {
            rand = new Random();
        }

        public double newRandomDouble(double span, double initialValue)
        {

            return rand.NextDouble() * span + initialValue;
        }

        public List<double> newRandomListOfDoubles(double span, double initialValue, int numOfNumbers)
        {
            List<double> values = new List<double>();
            for (int i = 0; i < numOfNumbers; i++)
            {
                values.Add(newRandomDouble(span,initialValue));
            }
            return values;
        }

        public Matrix<E> newRandomMatrix<C, E>(C collection, double span, double initialValue, int numOfNumbers)
              where C : IEnumerable<E>
        {
            Matrix<E> matrix = new Matrix<E>();

            foreach (E element in collection)
            {
                matrix.addElement(element, newRandomListOfDoubles(span,initialValue,numOfNumbers));
            }

            return matrix;
        }

        public Dictionary<E, double> newRandomVector<C, E>(C collection, double span, double initialValue)
            where C : IEnumerable<E>
        {
            Dictionary<E, double> vector = new Dictionary<E, double>();

            foreach (E element in collection)
            {
                vector.Add(element, newRandomDouble(span, initialValue));
            }

            return vector;
        }

        public Dictionary<string, double> NewRandomItemsPerUser(User user)
        {
            Dictionary<string, double> randomItems = new Dictionary<string, double>();
            int count = 0; //set the number of selected items per user

            int kItems = rand.Next(1, user.GetRatedItems().Count); //pick a random items from the user rating list

            while (count < kItems)
            {
                int idx = rand.Next(user.GetRatedItems().Count - 1);
                string itemId = user.GetRatedItems().ElementAt(idx);
                double rating = user.GetRating(itemId);
                randomItems.Add(itemId, rating);
                count++;

                user.RemoveItemById(itemId);
            }

            return randomItems;
        }

        public User newRandomUser(Users users)
        {
            var next = rand.Next(users.Count() - 1);
            return users.ElementAt(next);
        }
    }
}
