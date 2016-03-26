using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment1
{
    class SimilarityEngine
    {
        private Users users;
        private ILogger logger;
        private int MAX_SIMILAR_USERS;

        public SimilarityEngine(Users users ,int maxSimilarUsers , ILogger logger)
        {
            this.users = users;
            this.logger = logger;
            this.MAX_SIMILAR_USERS = maxSimilarUsers;
        }

        public Dictionary<double, User> calculateSimilarity(IPredictionMethod predictionMethod, User thisUser)
        {
            if (predictionMethod == null || thisUser == null)
                throw new ArgumentNullException("IPredictionMethod predictionMethod, User thisUser must both be not null!");

            logger.info("calcualting similarity for user " + "[" + thisUser.GetId() + "]");
            Stopwatch timer = Stopwatch.StartNew();
            BoundedSortedDictionary<double, User> similarUsers = new BoundedSortedDictionary<double, User>(MAX_SIMILAR_USERS);

            foreach (var thatUser in users)
            {
                double similarity;
                if (!thisUser.Equals(thatUser))
                {
                    var commonItems = thatUser.GetRatedItems().Intersect(thisUser.GetRatedItems()); //check if both users rated at least one common item 
                    if (commonItems.Any())
                    {
                        similarity = predictionMethod.calculateSimilarity(thisUser, thatUser);
                        if (similarity != 0) //in some cases the users rate their common item the same as their average then we can get here zero
                        {
                            similarUsers.add(similarity, thatUser);
                        }
                    }
                }
            }

            //Parallel.ForEach(users, thatUser =>
            //{
            //    double similarity;
            //    if (!thisUser.Equals(thisUser))
            //    {
            //        var commonItems = thatUser.GetRatedItems().Intersect(thisUser.GetRatedItems()); //check if both users rated at least one common item 
            //        if (commonItems.Any())
            //        {
            //            //logger.debug("calcualting similarity for user " + "[" + thisUser.GetId().ToString() + "]" + " with user " + "[" + thatUser.GetId().ToString() + "]");
            //            similarity = predictionMethod.calculateSimilarity(thisUser, thatUser);
            //            if (similarity != 0) //in some cases the users rate their common item the same as their average then we can get here zero
            //            {
            //                similarUsers.add(similarity, thatUser); 
            //            }
            //        }
            //    }
            //});
            timer.Stop();
            logger.debug("Similarity calculation time for user" + " [" + thisUser.GetId() + "] " + timer.Elapsed);

            return similarUsers.getSimilarUsers();
        }
    }
}
