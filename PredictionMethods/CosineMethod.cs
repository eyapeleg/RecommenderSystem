using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class CosineMethod : IPredictionMethod
    {
        Items items;
        Users users;

        public CosineMethod(Users users, Items items)
        {
            this.users = users;
            this.items = items;
        }
        
        //internal void calculateCosineSimilarity()
        //{
        //    Console.WriteLine("Calculate Cosine Similarity...");
        //    var userlist = users.GetAllUsers().Distinct();
        //    for (int i = 0; i < userlist.Count(); i++)
        //    {
        //        User u1 = users.getUserById(userlist.ElementAt(i));
        //        for (int j = i + 1; j < userlist.Count(); j++)
        //        {
        //            User u2 = users.getUserById(userlist.ElementAt(j));
        //            if(u1.getRatedItems().Intersect(u2.getRatedItems()).Count() == 0) //skip users who didn't rank the same items
        //            {
        //                continue;
        //            }
        //            double sim = calculateSimilarity(u1, u2);
        //            u1.setSimilarUser(u2, sim);
        //            u2.setSimilarUser(u1, sim);
        //        }
        //    }
        //    Console.WriteLine("Cosine similarity was completed successfully...");

        //}

        public double calculateSimilarity(User u1, User u2)
        {
            double dotProduct = 0.0;

            //Get the common rated items and calcuate dotProduct for them
            var intersectList = u1.GetRatedItems().Intersect(u2.GetRatedItems());
            foreach (var item in intersectList)
            {
                double u1Rating = u1.GetRating(item);
                double u2Rating = u2.GetRating(item);
                dotProduct += u1Rating * u2Rating;
            }
            if (intersectList.Count() == 0)
                return 0;

            double cos = dotProduct / (Math.Sqrt(u1.GetSquaredSum()) * (Math.Sqrt(u2.GetSquaredSum())));
            return cos;
        }

        public PredictionMethod GetPredictionMethod()
        {
            return  PredictionMethod.Cosine;
        }

        //internal double PredictRating(User user, string sIID)
        //{
        //    double rating = 0.0;
        //    var similiarUsers = user.getSimilarUser(PredictionMethod.Cosine);
        //    int norm = 0;

        //    for (int i = 0; i < similiarUsers.Count(); i++)
        //    {
        //        User simUser = similiarUsers.ElementAt(i).Key;
        //        double r = simUser.getRating(sIID); //TODO - take only those who rate item sIID
        //        if (r != 0)
        //        {
        //            norm++;
        //            double w = similiarUsers.ElementAt(i).Value;
        //            rating += w * r;
        //        }
        //    }

        //    if (norm == 0)
        //        return 0;

        //    return rating / (double)norm;
        //}
    }
}
