using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class StereotypesModel : IPredictionModel
    {
        private RandomGenerator randomGenerator;
        private SimilarityEngine similarityEngine;
        private ISimilarityMethod similarityMethod;
        private double CENTROIDS_SIMILARITY_THRESHOLD;
        private int minimumRatingThreshold;
        private Users users;
        private Items items;
        private int cStereotypes;
        private int MAX_ITERATION;
        private Stereotypes stereotypes;
        private Dictionary<User, User> similarityDic;

        public StereotypesModel(SimilarityEngine similarityEngine, ISimilarityMethod similarityMethod, Users users, Items items, int cStereotypes)
        {
            this.randomGenerator = new RandomGenerator();
            this.similarityEngine = similarityEngine;
            this.similarityMethod = similarityMethod;
            this.CENTROIDS_SIMILARITY_THRESHOLD = 0.99; // double.Parse(ConfigurationManager.AppSettings["CentroidsSimilarityThreshold"]);
            this.MAX_ITERATION = 20; // int.Parse(ConfigurationManager.AppSettings["maxNumIterationStereotype"]);
            this.minimumRatingThreshold = 50;// int.Parse(ConfigurationManager.AppSettings["minimumRatingThreshold"]);
            this.users = users;
            this.items = items;
            this.cStereotypes = cStereotypes;
            similarityDic = new Dictionary<User, User>();
        }

        public Stereotypes newRandomCentroids(Users users, Items items, int cStereotypes)
        {
            Stereotypes stereotypes = new Stereotypes();

            //create random initial stereotype
            List<User> centroids = new List<User>();
            var subsetUsers = users.Where(x => x.GetRatedItems().Count > minimumRatingThreshold).ToList();
            centroids.Add(randomGenerator.getRandomUser(subsetUsers));
            
            double sumSquaredDistanceFromStereotypes;

            //create cStereotypes 
            for (int i = 1; i < cStereotypes; i++)
            {
                double farestDistanceFromStereotypes = 0.0;
                User farestUser=null;

                //find farest user from stereotypes
                foreach (User user in subsetUsers)
                {
                    if (centroids.Contains(user))
                        continue;

                    // calculate similarites and update farest centroid accordingly
                    sumSquaredDistanceFromStereotypes = 0.0;
                    List<KeyValuePair<User, double>> disSimilarities = similarityEngine.calculateSimilarity(similarityMethod,user,centroids,true);

                    foreach(KeyValuePair<User, double> disSimilarity in disSimilarities)    {
                        sumSquaredDistanceFromStereotypes += Math.Pow(Math.Abs(disSimilarity.Value), 2);
                    }

                    if (sumSquaredDistanceFromStereotypes > farestDistanceFromStereotypes)
                    {
                        farestDistanceFromStereotypes = sumSquaredDistanceFromStereotypes;
                        farestUser = user;
                    }
                }
                if (farestUser == null)
                    throw new ArgumentNullException();

                centroids.Add(farestUser);
            }

            foreach(User user in centroids){
                stereotypes.addStereotype(new Stereotype(user));
            }
            return stereotypes;
        }

        public void Train()
        {
            Console.WriteLine("***************** Train Stereotypes  Model *********************");
            stereotypes = newRandomCentroids(users, items, cStereotypes);

            Users usersCopy = new Users(users);
            bool isConverged = false;
            int iteration = 1;

            while (!isConverged && iteration <= MAX_ITERATION)
            {

                Console.WriteLine("Iteration: {0}", iteration);
                List<User> prevCentroids = stereotypes.getStereotypesCentroids();
                stereotypes.initStereotypesUsers();
                List<User> usersWithNoSimilarity = new List<User>(); ;
                foreach (User user in usersCopy)
                {
                    //Calculate a users similarity to the stereotypes
                    List<KeyValuePair<User, double>> similarities = similarityEngine.calculateSimilarity(similarityMethod, user, prevCentroids, false, true); //TODO - check the similarity threshold for stereotype algorithm

                    //Determine users that don't have a correlation with non of the stereotypes
                    if (similarities.Count == 0)
                    {
                        usersWithNoSimilarity.Add(user);
                        continue;
                    }

                    //Allocate a user to it's most similar stereotype 
                    User mostSimilarCentroid = similarities.Last().Key;
                    stereotypes.addUserToStereotype(mostSimilarCentroid.GetId(), user);
                }
                usersCopy.removeUsers(usersWithNoSimilarity);

                //Determine convergence
                stereotypes.reCalculateCentroids();
                List<User> currCentroids = stereotypes.getStereotypesCentroids();
                var prevAndCurrCentroidsSimilarity = currCentroids.Zip(prevCentroids,(x, y) => Tuple.Create(x, y)).Select(x => Tuple.Create(x.Item1,x.Item2,similarityEngine.calculateSimilarity(similarityMethod, x.Item1, x.Item2))).ToList();
                if (prevAndCurrCentroidsSimilarity.All(x => x.Item3 > CENTROIDS_SIMILARITY_THRESHOLD))
                {
                    isConverged = true;
                }
                else
                {
                    iteration++;
                }
            }
        }

        public double Predict(User user, Item item){

            double rating = 0;
            List<User> centroids = stereotypes.getStereotypesCentroids();
            string itemId = item.GetId();

            if(!similarityDic.ContainsKey(user))
            {
                List<KeyValuePair<User, double>> similarities = similarityEngine.calculateSimilarity(similarityMethod, user, centroids, false, true);

                if(similarities.Count == 0)
                {
                    similarityDic.Add(user, null); //there is no similar centroid
                    rating = user.GetAverageRatings();
                    return rating;
                }

                //add the most similar user to the dictionary
                User mostSimilarUser = similarities.Last().Key;
                similarityDic.Add(user, mostSimilarUser);
                if (mostSimilarUser.GetRating(itemId) == 0) //if the most similar user didn't rate the item, return the average rating of that item
                {
                    rating = item.GetAverageRatings();
                    return rating;
                }
            }
            
            //if we didn't find any similar centroid, the average rating of the user is return
            if (similarityDic[user] == null)
            {
                return user.GetAverageRatings();
            }

            var similarUserRating = similarityDic[user].GetRating(itemId);
            return similarUserRating == 0 ? item.GetAverageRatings() : similarUserRating;
        }
    }
}
