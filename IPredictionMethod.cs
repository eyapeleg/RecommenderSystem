using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    interface IPredictionMethod
    {
        double calculateSimilarity(PredictionVector xVector, PredictionVector yVector);
    }
}
