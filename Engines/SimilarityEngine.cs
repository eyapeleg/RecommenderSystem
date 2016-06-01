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
        private Dictionary<ISimilarityMethod, Dictionary<User, List<KeyValuePair<User, double>>>> similarityDic;
        private Dictionary<ISimilarityMethod, Dictionary<User, Dictionary<User, double>>> similarityNNDic; 

        public SimilarityEngine(Users users ,int maxSimilarUsers , ILogger logger)
        {
            this.users = users;
            this.logger = logger;
            MAX_SIMILAR_USERS = maxSimilarUsers;
            similarityThreshold = 0.1;// Double.Parse(ConfigurationManager.AppSettings["similarityThreshold"]); //TODO why the automated tests failed on that
            commonItemsThreshold = 1; // Int32.Parse(ConfigurationManager.AppSettings["commonItemsThreshold"]);//TODO why the automated tests failed on that
            similarityDic = new Dictionary<ISimilarityMethod, Dictionary<User, List<KeyValuePair<User, double>>>>();
            similarityNNDic = new Dictionary<ISimilarityMethod, Dictionary<User, Dictionary<User, double>>>();
        }

        public List<KeyValuePair<User, double>> calculateSimilarity(ISimilarityMethod predictionMethod, User thisUser, List<string> candidateUsersIds)
        {
            if(!similarityDic.ContainsKey(predictionMethod))
            {
                similarityDic.Add(predictionMethod, new Dictionary<User, List<KeyValuePair<User, double>>>());
            }

            List<User> candidateUsers = candidateUsersIds.Select(userId => users.getUserById(userId)).Where(user => user != null).ToList(); 
            return calculateSimilarity(predictionMethod, thisUser, candidateUsers);
        }

        public double calculateSimilarity(ISimilarityMethod predictionMethod, User thisUser, User candidateUser)
        {
            double similarity = 0;

            if (!similarityNNDic.ContainsKey(predictionMethod))
            {
                similarityNNDic.Add(predictionMethod, new Dictionary<User, Dictionary<User, double>>());
            }

            if (!similarityNNDic[predictionMethod].ContainsKey(thisUser))
            {
                similarityNNDic[predictionMethod].Add(thisUser, new Dictionary<User, double>());
            }

           
            if (similarityNNDic[predictionMethod][thisUser].ContainsKey(candidateUser))
                return similarityNNDic[predictionMethod][thisUser][candidateUser];

            List<string> thisUserList = thisUser.GetRatedItems();
            List<string> commonItemsList = candidateUser.GetRatedItems().Intersect(thisUserList).ToList();
            if (commonItemsList.Count > commonItemsThreshold)
            {
                similarity = predictionMethod.calculateSimilarity(thisUser, candidateUser, commonItemsList);
            }
            
            similarityNNDic[predictionMethod][thisUser].Add(candidateUser, similarity);
                
            return similarity;
        }
       
        public List<KeyValuePair<User, double>> calculateSimilarity(ISimilarityMethod predictionMethod, User thisUser, List<User> candidateUsers, bool revertSimilarities = false)
        {
            if (predictionMethod == null || thisUser == null)
                throw new ArgumentNullException("IPredictionMethod predictionMethod, User thisUser must both be not null!");

            if (!similarityDic.ContainsKey(predictionMethod))
            {
                similarityDic.Add(predictionMethod, new Dictionary<User,List<KeyValuePair<User,double>>>());
            }

            if (similarityDic[predictionMethod].ContainsKey(thisUser))
            {
                return similarityDic[predictionMethod][thisUser];
            }

            logger.debug("calcualting similarity for user " + "[" + thisUser.GetId() + "]");
            Stopwatch timer = Stopwatch.StartNew();
            UsersSimilarity similarUsers = new UsersSimilarity(MAX_SIMILAR_USERS);
            var thisUserList = thisUser.GetRatedItems();
            var similarUsersCount = 0;

            foreach (var thatUser in candidateUsers)
            {
                if (thatUser != null && !thisUser.Equals(thatUser))
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
                            similarUsersCount++;
                        }
                    }      
                }
            }

            timer.Stop();
            logger.debug("Similarity calculation time for user" + " [" + thisUser.GetId() + "] " + timer.Elapsed);

            similarityDic[predictionMethod].Add(thisUser, similarUsers.AsList()); //Eyal return descending sorted list here

            return similarUsers.AsList();
        }
    }
}
