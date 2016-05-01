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

        public StereotypesModel(SimilarityEngine similarityEngine, ISimilarityMethod similarityMethod, Users users, Items items, int cStereotypes)
        {
            this.randomGenerator = new RandomGenerator();
            this.similarityEngine = similarityEngine;
            this.similarityMethod = similarityMethod;
            this.CENTROIDS_SIMILARITY_THRESHOLD = double.Parse(ConfigurationManager.AppSettings["CentroidsSimilarityThreshold"]);
            this.MAX_ITERATION = int.Parse(ConfigurationManager.AppSettings["maxNumIterationStereotype"]);
            this.minimumRatingThreshold = int.Parse(ConfigurationManager.AppSettings["minimumRatingThreshold"]);
            this.users = users;
            this.items = items;
            this.cStereotypes = cStereotypes;
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
                    List<KeyValuePair<User, double>> similarities = similarityEngine.calculateSimilarity(similarityMethod,user,centroids);

                    foreach(KeyValuePair<User, double> similarity in similarities)    {
                        sumSquaredDistanceFromStereotypes += Math.Pow(1-Math.Abs(similarity.Value), 2);
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
                subsetUsers.Remove(farestUser); //TODO Eyal- why he didn't remove the farest user from the subset list once it's added to the centroids
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
                    List<KeyValuePair<User, double>> similarities = similarityEngine.calculateSimilarity(similarityMethod, user, prevCentroids); //TODO Eyal - check the similarity threshold for stereotype algorithm

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
            List<User> centroids = stereotypes.getStereotypesCentroids();
            List<KeyValuePair<User, double>> similarities = similarityEngine.calculateSimilarity(similarityMethod, user, centroids);
            string itemId = item.GetId();

            if (similarities.Count == 0)
            {
                //var avgRating = centroids.Average(x => x.GetRating(itemId)); //Change to return instead of variable declaration 
                return user.GetAverageRatings();
            }

            User mostSimilarUser = similarities.Last().Key;

            if (mostSimilarUser.GetRating(itemId) == 0.0)
            {
                //TODO Eyal - 1.why not to try the 2nd level similarity centroid? , 2.consider the return value for case the item didn't found
                return item.GetAverageRatings(); 
            }

            return mostSimilarUser.GetRating(itemId);
        }
    }
}
