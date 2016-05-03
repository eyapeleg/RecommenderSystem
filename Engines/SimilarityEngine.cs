using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RecommenderSystem
{
    public class SimilarityEngine
    {
        private Users users;
        private ILogger logger;
        private int MAX_SIMILAR_USERS;
        private readonly double similarityThreshold;
        private readonly int commonItemsThreshold;

        public SimilarityEngine(Users users ,int maxSimilarUsers , ILogger logger)
        {
            this.users = users;
            this.logger = logger;
            MAX_SIMILAR_USERS = maxSimilarUsers;
            similarityThreshold = 0.1;// Double.Parse(ConfigurationManager.AppSettings["similarityThreshold"]); //TODO why the automated tests failed on that
            commonItemsThreshold = 1; // Int32.Parse(ConfigurationManager.AppSettings["commonItemsThreshold"]);//TODO why the automated tests failed on that
        }

        public List<KeyValuePair<User, double>> calculateSimilarity(ISimilarityMethod predictionMethod, User thisUser, List<string> candidateUsersIds)
        {
            List<User> candidateUsers = candidateUsersIds.Select(userId => users.getUserById(userId)).Where(user => user != null).ToList(); 
            return calculateSimilarity(predictionMethod, thisUser, candidateUsers);
        }

        public double calculateSimilarity(ISimilarityMethod predictionMethod, User thisUser, User candidateUser)
        {
            List<KeyValuePair<User, double>> similarities = calculateSimilarity(predictionMethod, thisUser, new List<User>(){candidateUser});
            if (similarities.Count == 0)
                return 0.0;
            return similarities[0].Value;
        }
       
        public List<KeyValuePair<User, double>> calculateSimilarity(ISimilarityMethod predictionMethod, User thisUser, List<User> candidateUsers, bool revertSimilarities = false)
        {
            if (predictionMethod == null || thisUser == null)
                throw new ArgumentNullException("IPredictionMethod predictionMethod, User thisUser must both be not null!");

            logger.debug("calcualting similarity for user " + "[" + thisUser.GetId() + "]");
            Stopwatch timer = Stopwatch.StartNew();
            UsersSimilarity similarUsers = new UsersSimilarity(MAX_SIMILAR_USERS);
            var thisUserList = thisUser.GetRatedItems();

            foreach (var thatUser in candidateUsers)
            {
                if (thatUser != null && !thisUser.Equals(thatUser))
                {
                    try
                    {
                        List<string> thatUserList = thatUser.GetRatedItems();
                        List<string> commonItemsList = thatUserList.Intersect(thisUserList).ToList(); //check if both users rated at least one common item 

                        if (commonItemsList.Count > commonItemsThreshold && thatUserList.Count > 0 && thisUserList.Count > 0)
                        {
                            var similarity = predictionMethod.calculateSimilarity(thisUser, thatUser, commonItemsList);
                            if (revertSimilarities == true)
                            {
                                similarity *= -1;
                            }
                            if (similarity > similarityThreshold) //in some cases the users rate their common item the same as their average then we can get here zero
                            {
                                similarUsers.add(thatUser, similarity);
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine("user does not exist"); //TODO remove - only for interanl testing
                        continue;
                    }         
                }
            }
            timer.Stop();
            logger.debug("Similarity calculation time for user" + " [" + thisUser.GetId() + "] " + timer.Elapsed);

            return similarUsers.AsList();
        }
    }
}
