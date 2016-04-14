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
    }
}
