using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public static class MathUtils
    {
        public static List<double> MultipleScalarByVector(double scalar, List<double> vector)
        {
            return vector.Select(x => x * scalar).ToList();
        }

        public static List<double> MinusVectors(List<double> l1, List<double> l2)
        {
            List<double> result = new List<double>();

            if (l1.Count != l2.Count)
            {
                throw new ArgumentException("Vector lengths different");
            }

            return l1.Select((t, i) => t - l2[i]).ToList();
        }

        public static List<double> AdditionVectors(List<double> l1, List<double> l2)
        {
            if (l1.Count != l2.Count)
            {
                throw new ArgumentException("Vector lengths different");
            }

            return l1.Select((t, i) => t + l2[i]).ToList();
        }
    }
}
