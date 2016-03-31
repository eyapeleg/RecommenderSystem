﻿using System;
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
            similarityThreshold = Double.Parse(ConfigurationManager.AppSettings["similarityThreshold"]);
            commonItemsThreshold = Int32.Parse(ConfigurationManager.AppSettings["commonItemsThreshold"]);
        }

        public List<KeyValuePair<User, double>> calculateSimilarity(IPredictionMethod predictionMethod, User thisUser, List<string> candidateUsers)
        {
            if (predictionMethod == null || thisUser == null)
                throw new ArgumentNullException("IPredictionMethod predictionMethod, User thisUser must both be not null!");

            logger.debug("calcualting similarity for user " + "[" + thisUser.GetId() + "]");
            Stopwatch timer = Stopwatch.StartNew();
            UsersSimilarity similarUsers = new UsersSimilarity(MAX_SIMILAR_USERS);
            var thisUserList = thisUser.GetRatedItems();

            foreach (var user in candidateUsers)
            {
                User thatUser = users.getUserById(user);
                if (!thisUser.Equals(thatUser))
                {
                    var thatUserList = thatUser.GetRatedItems();

                    List<string> commonItemsList = thatUserList.Intersect(thisUserList).ToList(); //check if both users rated at least one common item 
                    //TODO decide about threshold
                    if (commonItemsList.Count > commonItemsThreshold && thatUserList.Count > 0 && thisUserList.Count > 0)
                    {
                        var similarity = predictionMethod.calculateSimilarity(thisUser, thatUser, commonItemsList);
                        if (similarity > similarityThreshold) //in some cases the users rate their common item the same as their average then we can get here zero
                        {
                            similarUsers.add(thatUser, similarity);
                        }
                    }
                }
            }

            //Parallel.ForEach(users, thatUser =>
            //{
            //    double similarity;
            //    if (!thatUser.Equals(thisUser))
            //    {
            //        List<string> commonItems = thatUser.GetRatedItems().Intersect(thisUser.GetRatedItems()).ToList(); //check if both users rated at least one common item 
            //        if (commonItems.Any())
            //        {
            //            logger.debug("calcualting similarity for user " + "[" + thisUser.GetId().ToString() + "]" + " with user " + "[" + thatUser.GetId().ToString() + "]");
            //            similarity = predictionMethod.calculateSimilarity(thisUser, thatUser, commonItems);
            //            if (similarity != 0) //in some cases the users rate their common item the same as their average then we can get here zero
            //            {
            //                similarUsers.add(thatUser, similarity);
            //            }
            //        }
            //    }
            //});
            timer.Stop();
            logger.debug("Similarity calculation time for user" + " [" + thisUser.GetId() + "] " + timer.Elapsed);

            return similarUsers.AsList();
        }
    }
}
