using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using RecommenderSystem;

namespace RecommenderSystem
{
    public interface IPredictionModel
    {
        void Train();
        double Predict(User user, Item item);
    }
}
