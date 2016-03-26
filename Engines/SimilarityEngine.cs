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

            logger.info("calcualting similarity for user " + "[" + thisUser.GetId().ToString() + "]");
            Stopwatch timer = Stopwatch.StartNew();
            BoundedSortedDictionary<double, User> similarUsers = new BoundedSortedDictionary<double, User>(MAX_SIMILAR_USERS);

            Parallel.ForEach<User>(users, thatUser =>
            {
                if (!thisUser.Equals(thisUser)){
                    //logger.debug("calcualting similarity for user " + "[" + thisUser.GetId().ToString() + "]" + " with user " + "[" + thatUser.GetId().ToString() + "]");
                    double similarity = predictionMethod.calculateSimilarity(thisUser, thatUser);
                    if (similarity != 0)
                    {
                        similarUsers.add(similarity, thatUser);
                    }
                }
            });
            timer.Stop();
            logger.debug("Similarity calculation time for user" + " [" + thisUser.GetId().ToString() + "] " + timer.Elapsed.ToString());

            return similarUsers.getSimilarUsers();
        }
    }
}
