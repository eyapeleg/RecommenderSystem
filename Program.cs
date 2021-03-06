﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    class Program
    {


        static void Assignment3()
        {
            RecommenderSystem rs = new RecommenderSystem();

            rs.Load("ratings.dat", 0.97);
            //rs.TrainBaseModel(10);
            //rs.TrainStereotypes(10);

            string testUserId = rs.getTestUserId();
            //List<string> lRecommendationsCp = rs.Recommend(RecommenderSystem.RecommendationMethod.CP, testUserId, 5);
            //List<string> lRecommendationsJaccard = rs.Recommend(RecommenderSystem.RecommendationMethod.Jaccard, testUserId, 5);

            //List<string> lRecommendations = rs.Recommend(RecommenderSystem.RecommendationMethod.Pearson, "6", 5);
            //Console.Write("Recommended movies for user 6 ");
            //foreach (string sMovie in lRecommendations)
            //    Console.Write(sMovie + ",");
            //Console.WriteLine();
            //
            //List<string> lRecommendationsPopulairty = rs.Recommend(RecommenderSystem.RecommendationMethod.Popularity, "2", 5);
            //List<string> lRecommendationsPearson = rs.Recommend(RecommenderSystem.RecommendationMethod.Pearson, "2", 5);
            //List<string> lRecommendationsCosine = rs.Recommend(RecommenderSystem.RecommendationMethod.Cosine, "2", 5);
            //List<string> lRecommendationsBaseModel = rs.Recommend(RecommenderSystem.RecommendationMethod.BaseModel, "2", 5);
            //List<string> lRecommendationsSt = rs.Recommend(RecommenderSystem.RecommendationMethod.Stereotypes, "2", 5);

            //
            List<RecommenderSystem.RecommendationMethod> lMethods = new List<RecommenderSystem.RecommendationMethod>();


            //List<RecommenderSystem.RecommendationMethod> lMethods = new List<RecommenderSystem.RecommendationMethod>();


            lMethods.Add(RecommenderSystem.RecommendationMethod.Jaccard);
            lMethods.Add(RecommenderSystem.RecommendationMethod.CP);
            //
            List<int> lLengths = new List<int>();
            lLengths.Add(1);
            lLengths.Add(3);
            lLengths.Add(5);
            lLengths.Add(10);
            lLengths.Add(20);

            DateTime dtStart = DateTime.Now;
            Dictionary<int, Dictionary<RecommenderSystem.RecommendationMethod, Dictionary<string, double>>> dResults = rs.ComputePrecisionRecall(lMethods, lLengths, 1000);
            Console.WriteLine("Precision-recall scores for all methods are:");
            foreach (int iLength in lLengths)
            {
                foreach (RecommenderSystem.RecommendationMethod sMethod in lMethods)
                {
                    foreach (string sMetric in dResults[iLength][sMethod].Keys)
                    {
                        Console.WriteLine(iLength + "," + sMethod + "," + sMetric + " = " + Math.Round(dResults[iLength][sMethod][sMetric], 4));
                    }
                }
            }
            Console.WriteLine("Execution time was " + Math.Round((DateTime.Now - dtStart).TotalSeconds, 0));
            Console.ReadLine();
        }


        static void Main(string[] args)
        {
            Assignment3();
        }
    }
}
