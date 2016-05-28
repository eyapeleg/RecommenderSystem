using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class RandomGenerator
    {
        private Random rand;
        //double minItemsToTake = Double.Parse(ConfigurationManager.AppSettings["split_minItemsToTake"]); 
        double minItemsToTake = 3;

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
            List<string> ratingItems = user.GetRatedItems().Select(item => (string)item.Clone()).ToList();
            int count = 0; //set the number of selected items per user           

            //TODO Eyal- consider to take at least half of the rated items
            int kItems = rand.Next((int)minItemsToTake, ratingItems.Count - 1); //pick a random items from the user rating list - limit the total number of selected items

            while (count < kItems)
            {
                int idx = rand.Next(ratingItems.Count - 1);
                string itemId = ratingItems.ElementAt(idx);
                double rating = user.GetRating(itemId);
                randomItems.Add(itemId, rating);
                count++;

                ratingItems.Remove(itemId);
            }

            return randomItems;
        }

        public User getRandomUser(IEnumerable<User> users)
        {
            var next = rand.Next(users.Count() - 1);
            return users.ElementAt(next);
        }
    }
}
