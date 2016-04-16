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

        public StereotypesEngine(SimilarityEngine similarityEngine, IPredictionMethod predictionMethod)
        {
            this.randomGenerator = new RandomGenerator();
            this.similarityEngine = similarityEngine;
            this.predictionMethod = predictionMethod;
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
                    if (user.GetRatedItems().Count<20 || centroids.Contains(user)) //TODO - parameterize this value
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
                stereotypes.addStereotype(new Stereotype(new User(user)));
            }
            return stereotypes;
        }

        public void trainStereotypes(Stereotypes stereotypes, Users users)
        {
            // add iterations for convergence
            List<User> centroids = stereotypes.getStereotypesCentroids();
            foreach (User user in users)
            {
                List<KeyValuePair<User, double>> similarities = similarityEngine.calculateSimilarity(predictionMethod, user, centroids);
                
                double highestSimilarity = 0.0;
                User mostSimilarCentroid=null;
                foreach(KeyValuePair<User, double> similarity in similarities){
                    if (similarity.Value>highestSimilarity){
                        highestSimilarity = similarity.Value;
                        mostSimilarCentroid = similarity.Key;
                    }
                }
                if (mostSimilarCentroid == null)
                    throw new ArgumentNullException();
                stereotypes.addUserToStereotype(mostSimilarCentroid.GetId(), user);
            }
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
