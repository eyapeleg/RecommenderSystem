using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class StereotypesEngine
    {
        private RandomGenerator randomGenerator;
        private SimilarityEngine similarityEngine;
        private IPredictionMethod predictionMethod;
        private double CENTROIDS_SIMILARITY_THRESHOLD;

        public StereotypesEngine(SimilarityEngine similarityEngine, IPredictionMethod predictionMethod)
        {
            this.randomGenerator = new RandomGenerator();
            this.similarityEngine = similarityEngine;
            this.predictionMethod = predictionMethod;
            this.CENTROIDS_SIMILARITY_THRESHOLD = CENTROIDS_SIMILARITY_THRESHOLD = 0.99; //TODO - get from an external file
        }

        public Stereotypes initStereotypes(Users users, Items items, int cStereotypes)
        {
            Stereotypes stereotypes = new Stereotypes();

            //create random initial stereotype
            List<User> centroids = new List<User>();
            centroids.Add(randomGenerator.getValidRandomUser(users));
            
            double sumSquaredDistanceFromStereotypes;

            //create cStereotypes 
            for (int i = 1; i < cStereotypes; i++)
            {
                double farestDistanceFromStereotypes = 0.0;
                User farestUser=null;

                //find farest user from stereotypes
                foreach (User user in users)
                {
                    if (user.GetRatedItems().Count<50 || centroids.Contains(user)) //TODO - parameterize this value
                        continue;

                    // calculate similarites and update farest centroid accordingly
                    sumSquaredDistanceFromStereotypes =0.0;
                    List<KeyValuePair<User, double>> similarities = similarityEngine.calculateSimilarity(predictionMethod,user,centroids)   ; 
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
            }

            foreach(User user in centroids){
                stereotypes.addStereotype(new Stereotype(user));
            }
            return stereotypes;
        }

        public Stereotypes trainStereotypes(Stereotypes stereotypes, Users users)
        {
            Users usersCopy = new Users(users);
            bool isConverged = false;

            while(!isConverged){//TODO - add number of iterations threshold convergence

                List<User> prevCentroids = stereotypes.getStereotypesCentroids();
                stereotypes.initStereotypesUsers();
                List<User> usersWithNoSimilarity = new List<User>(); ;
                foreach (User user in usersCopy)
                {
                    //Calculate a users similarity to the stereotypes
                    List<KeyValuePair<User, double>> similarities = similarityEngine.calculateSimilarity(predictionMethod, user, prevCentroids);

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
                var prevAndCurrCentroidsSimilarity = currCentroids.Zip(prevCentroids,(x, y) => Tuple.Create(x, y)).Select(x => Tuple.Create(x.Item1,x.Item2,similarityEngine.calculateSimilarity(predictionMethod, x.Item1, x.Item2))).ToList();
                if (prevAndCurrCentroidsSimilarity.All(x => x.Item3>CENTROIDS_SIMILARITY_THRESHOLD))
                    isConverged=true;
            }

            return stereotypes;
        }

        public double predict(Stereotypes stereotypes, User user, string itemId){
            List<User> centroids = stereotypes.getStereotypesCentroids();
            List<KeyValuePair<User, double>> similarities = similarityEngine.calculateSimilarity(predictionMethod, user, centroids);

            if (similarities.Count == 0)
            {
                throw new NotImplementedException(); //TODO 
            }

            User mostSimilarUser = similarities.Last().Key;
            return mostSimilarUser.GetRating(itemId);
        }


        /*private Stereotype generateStereotypeFromUser(User user, Items items)
        {
            User newUser = new User(user);
            List<Item> ratedItems = newUser.GetRatedItems().Select(itemID => items.GetItemById(itemID)).ToList();
            List<Item> unratedItems = items.GetAllItems().Except(ratedItems).ToList();
            foreach (Item item in unratedItems)
            {
                newUser.AddItem(item.GetId(), 0.0); //TODO - verify that 0.0 is the right value in this case;
            }

            return new Stereotype(newUser);
        }*/

    }
}
