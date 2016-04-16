using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class MatrixUtils
    {
        public List<double> MultipleScalarByVector(double scalar, List<double> vector)
        {
            for (int i = 0; i < vector.Count; i++)
            {
                vector[i] *= scalar;
            }

            return vector;
        }

        public List<double> MinusVectors(List<double> l1, List<double> l2)
        {
            List<double> result = new List<double>();

            if (l1.Count != l2.Count)
            {
                throw new NotImplementedException();
            }

            for (int i = 0; i < l1.Count; i++)
            {
                result[i] = l1[i] - l2[i];
            }

            return result;
        }

        public List<double> AdditionVectors(List<double> l1, List<double> l2)
        {
            List<double> result = new List<double>();

            if (l1.Count != l2.Count)
            {
                throw new NotImplementedException();
            }

            for (int i = 0; i < l1.Count; i++)
            {
                result[i] = l1[i] + l2[i];
            }

            return result;
        }
    }
}
