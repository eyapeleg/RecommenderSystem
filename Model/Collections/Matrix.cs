using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class Matrix<T>
    {
        Dictionary<T, List<double>> matrix;

        public Matrix()
        {
            matrix = new Dictionary<T, List<double>>();
        }

        public void addElement(T element, List<double> values)
        {
            matrix.Add(element, values);
        }

        public List<double> getElementAt(T element)
        {
            List<double> values;
            matrix.TryGetValue(element, out values);

            if (values == null)
            {
                throw new ArgumentNullException("Element Not found!");
            }
            return values;
        }

        public void setElementAt(T element, List<double> values){
            if (!matrix.ContainsKey(element))
                throw new ArgumentNullException("Element Not found!");

            matrix[element] = values;
        }

    }
}
