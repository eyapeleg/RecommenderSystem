using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    class Program
    {


        static void Assignment2()
        {
            RecommenderSystem rs = new RecommenderSystem();
            rs.Load("ratings.dat", 0.95);
            rs.TrainBaseModel(10);
            rs.TrainStereotypes(10);
            List<RecommenderSystem.PredictionMethod> lMethods = new List<RecommenderSystem.PredictionMethod>();
            lMethods.Add(RecommenderSystem.PredictionMethod.BaseModel);
            lMethods.Add(RecommenderSystem.PredictionMethod.Stereotypes);
            lMethods.Add(RecommenderSystem.PredictionMethod.Pearson);
            lMethods.Add(RecommenderSystem.PredictionMethod.Cosine);
            lMethods.Add(RecommenderSystem.PredictionMethod.Random);
            DateTime dtStart = DateTime.Now;
            Dictionary<RecommenderSystem.PredictionMethod, double> dResults = rs.ComputeRMSE(lMethods);
            Console.WriteLine("Hit ratio scores for Pearson, Cosine, BaseModel, Stereotypes, and Random are:");
            foreach (KeyValuePair<RecommenderSystem.PredictionMethod, double> p in dResults)
                Console.Write(p.Key + "=" + Math.Round(p.Value, 4) + ", ");
            Console.WriteLine();
            Console.WriteLine("Execution time was " + Math.Round((DateTime.Now - dtStart).TotalSeconds, 0));
            Console.ReadLine();
        }

        static void Main(string[] args)
        {

            Assignment2();
        }
    }
}
