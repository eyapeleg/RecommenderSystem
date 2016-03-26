using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public interface IPredictionMethod
    {
        double calculateSimilarity(User u1, User u2, List<string> intersectList );

        PredictionMethod GetPredictionMethod();
    }
}
