using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class PearsonMethod : IPredictionMethod
    {
        public double calculateSimilarity(PredictionVector xVector, PredictionVector yVector)
        {
            double numeratorSum = 0.0;
            double denumeratorXsquareSum = 0.0;
            double denumeratorYsquareSum = 0.0;

            foreach(KeyValuePair<string,double> xPoint in xVector){
                if (!yVector.containsPoint(xPoint.Key))
                    break;

                double yPointValue = yVector.getPoint(xPoint.Key);
                double xPointValue = xPoint.Value;
                
                double xDelta = xPointValue-xVector.getAvg();
                double yDelta = yPointValue - yVector.getAvg(); 

                numeratorSum+= xDelta*yDelta;
                denumeratorXsquareSum +=  xDelta*xDelta;
                denumeratorYsquareSum +=  yDelta*yDelta;
            }

            if (denumeratorXsquareSum==0.0)
                throw new ArithmeticException("There is no intersection between the vectors");

            return numeratorSum/(denumeratorXsquareSum*denumeratorXsquareSum);
            }
               

        }
}

