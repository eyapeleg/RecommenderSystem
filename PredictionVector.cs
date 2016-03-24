using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class PredictionVector : IEnumerable<KeyValuePair<string, double>>
    {
        Dictionary<string, double> vector;
        double avg;
        int count;

        public PredictionVector(Dictionary<string, double> vector, double avg, int count)
        {
            this.vector = vector;
            this.avg = avg;
            this.count = count;
        }

        public int getCount()
        {
            return count;
        }

        public double getAvg()
        {
            return avg;
        }

        public IEnumerator<KeyValuePair<string,double>> GetEnumerator()
        {
            return vector.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return vector.GetEnumerator();
        }

        public bool containsPoint(string point)
        {
            return vector.ContainsKey(point);
        }

        public double getPoint(string point)
        {
            double pointValue;
            vector.TryGetValue(point, out pointValue);
            return pointValue;
        }



    }
}
