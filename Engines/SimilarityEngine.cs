using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment1
{
    public class SimilarityEngine
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

        public List<KeyValuePair<double, User>> calculateSimilarity(IPredictionMethod predictionMethod, User thisUser)
        {
            if (predictionMethod == null || thisUser == null)
                throw new ArgumentNullException("IPredictionMethod predictionMethod, User thisUser must both be not null!");

            logger.info("calcualting similarity for user " + "[" + thisUser.GetId().ToString() + "]");
            Stopwatch timer = Stopwatch.StartNew();
            BoundedSortedList<double, User> similarUsers = new BoundedSortedList<double, User>(MAX_SIMILAR_USERS);

            Parallel.ForEach<User>(users, thatUser =>
            {
                if (!thisUser.Equals(thatUser))
                {
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

            return similarUsers.getList();
        }
    }
}
