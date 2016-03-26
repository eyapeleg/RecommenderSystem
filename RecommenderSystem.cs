using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment1
{
    public class RecommenderSystem
    {
        Dictionary<PredictionMethod, IPredictionMethod> predictionMethodsDictionary;
        public enum PredictionMethod { Pearson, Cosine, Random };
        private Users users;
        private Items items;
        private SimilarityEngine similarityEngine;
        private DataLoaderEngine dataLoaderEngine;
        private PredictionEngine predictionEngine;
        private ILogger logger;

        private int MAX_SIMILAR_USERS;

        public RecommenderSystem()
        {
            logger = new InfoLogger();
            dataLoaderEngine = new DataLoaderEngine(logger);
            predictionEngine = new PredictionEngine();

            MAX_SIMILAR_USERS = 30;
            predictionMethodsDictionary = new Dictionary<PredictionMethod, IPredictionMethod>(){
                {PredictionMethod.Pearson,new PearsonMethod()},
                {PredictionMethod.Cosine, new PearsonMethod()}, //TODO - modify to the corresponding method
                {PredictionMethod.Random, new PearsonMethod()} //TODO - modify to the corresponding method
            };

            
        }

        public void Load(string sFileName)
        {
            
            Tuple<Users, Items> data = dataLoaderEngine.Load(sFileName);
            users = data.Item1;
            items = data.Item2;
            similarityEngine = new SimilarityEngine(users, MAX_SIMILAR_USERS, logger);
        }          

        //return a list of the ids of all the users in the dataset
        public List<string> GetAllUsers()
        {
            return users.GetAllUsers();
        }

        //returns a list of all the items in the dataset
        public List<string> GetAllItems()
        {
            return items.GetAllItems();
        }

        //returns the list of all items that the given user has rated in the dataset
        public List<string> GetRatedItems(string sUID)
        {
            return users.GetRatedItems(sUID);
        }

        //Returns a user-item rating that appears in the dataset (not predicted)
        public double GetRating(string sUID, string sIID)
        {
            return users.GetRating(sUID, sIID);
        }

        //predict a rating for a user item pair using the specified method
        public double PredictRating(PredictionMethod m, string sUID, string sIID)
        {
            
             User user = users.getUserById(sUID);

            if (!predictionMethodsDictionary.ContainsKey(m))
                throw new ArgumentException("Method "+"["+m.ToString()+"]"+" does not exist!" );

            IPredictionMethod predictionMethod= predictionMethodsDictionary[m];

            IList<KeyValuePair<double, User>> similarUsers = similarityEngine.calculateSimilarity(predictionMethod, user);
            return predictionEngine.predictRating(user, sIID, similarUsers);
        }

        //Compute MAE (mean absolute error) for a set of rating prediction methods over the same user-item pairs
        //cTrials specifies the number of user-item pairs to be tested
        public Dictionary<PredictionMethod, double> ComputeMAE(List<PredictionMethod> lMethods, int cTrials)
        {
            throw new NotImplementedException();
        }

    }
}
