using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Configuration;
using System.Threading.Tasks;

namespace RecommenderSystem
{
    public class RecommenderSystem
    {
        public enum PredictionMethod { Pearson, Cosine, Random, BaseModel, Stereotypes, Jaccard };
        public enum RecommendationMethod { Popularity, Pearson, Cosine, BaseModel, Stereotypes, NNPearson, NNCosine, NNBaseModel, NNJaccard, CP, Jaccard };

        public enum DatasetType { Train, Test, Validation};
        private Users users;
        private Items items;
        DataUtils dataUtils = new DataUtils();

        //TODO: decide how to formalize that part
        private Users trainUsers;
        private Items trainItems;
        private Users validationUsers;
        private Items validationItems;
        private Users testUsers;
        private Items testItems;

        private double averageTrainRating;

        private SimilarityEngine similarityEngine;
        private DataLoaderEngine dataLoaderEngine;
        private PredictionEngine predictionEngine;
        private EvaluationEngine evaluationEngine;
        private ConditionalProbabilityModel conditionalProbabilityModel;
        private ILogger logger;

        private int MAX_SIMILAR_USERS;
        private int NUM_OF_TRIALS;
        private int dsSize;
        private double TRAIN_DATA_PERCENTAGE;

        public RecommenderSystem()
        {
            logger = new InfoLogger();
            dataLoaderEngine = new DataLoaderEngine(logger);
            predictionEngine = new PredictionEngine();

            MAX_SIMILAR_USERS = 30;
            NUM_OF_TRIALS = 1000;

        }

        public void Load(string sFileName)
        {
            Tuple<Users, Items> data = dataLoaderEngine.Load(sFileName);
            users = data.Item1;
            items = data.Item2;
            similarityEngine = new SimilarityEngine(users, MAX_SIMILAR_USERS, logger);
            TRAIN_DATA_PERCENTAGE = 0.95; // double.Parse(ConfigurationManager.AppSettings["train_data_percentage"]);
        }

        public void Load(string sFileName, double dTrainSetSize)
        {
            Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>> splittedData;

            Tuple<Users, Items> data = dataLoaderEngine.Load(sFileName);
            users = data.Item1;
            items = data.Item2;
            
            this.dsSize = dataLoaderEngine.GetDataSetSize();
            splittedData  = dataUtils.Split(dTrainSetSize, dsSize, data, DatasetType.Test, DatasetType.Train);

            trainUsers = splittedData[DatasetType.Train].Item1;
            trainItems = splittedData[DatasetType.Train].Item2;
            testUsers = splittedData[DatasetType.Test].Item1;
            testItems = splittedData[DatasetType.Test].Item2;

            double trainSize = Math.Round(dsSize * dTrainSetSize);
            splittedData = dataUtils.Split(dTrainSetSize, trainSize, new Tuple<Users, Items>(this.trainUsers, this.trainItems), DatasetType.Validation, DatasetType.Train);

            trainUsers = splittedData[DatasetType.Train].Item1;
            trainItems = splittedData[DatasetType.Train].Item2;
            validationUsers = splittedData[DatasetType.Validation].Item1;
            validationItems = splittedData[DatasetType.Validation].Item2;

            //calculate the overall average rating 
            CalculateAverageRatingForTrainingSet();

            similarityEngine = new SimilarityEngine(testUsers, MAX_SIMILAR_USERS, logger);  //Similarity engine used by pearson and cosine 
            evaluationEngine = new EvaluationEngine(averageTrainRating);

            predictionEngine.addModel(PredictionMethod.Cosine, new CollaborativeFilteringModel(testUsers, testItems, similarityEngine, new CosineMethod()));
            predictionEngine.addModel(PredictionMethod.Pearson, new CollaborativeFilteringModel(testUsers, testItems, similarityEngine, new PearsonMethod()));
            predictionEngine.addModel(PredictionMethod.Random, new CollaborativeFilteringModel(testUsers, testItems, similarityEngine, new RandomMethod()));

            conditionalProbabilityModel = new ConditionalProbabilityModel(trainUsers, trainItems);
        }

        public void TrainBaseModel(int cFeatures)
        {
            double avgRating = getAverageRating(trainUsers); //calculate the average rating of the training set

            IPredictionModel matrixFactorizationModel = new MatrixFactorizationModel(trainUsers, trainItems, validationUsers, validationItems, cFeatures, avgRating);
            predictionEngine.addModel(PredictionMethod.BaseModel, matrixFactorizationModel);
            predictionEngine.Train(PredictionMethod.BaseModel);
        }

        public void TrainStereotypes(int cStereotypes)
        {
            IPredictionModel stereotypesModel = new StereotypesModel(similarityEngine, new PearsonMethod(), trainUsers, trainItems, cStereotypes);
            predictionEngine.addModel(PredictionMethod.Stereotypes, stereotypesModel);
            predictionEngine.Train(PredictionMethod.Stereotypes);
        }


        //return a list of the ids of all the users in the dataset
        public List<string> GetAllUsers()
        {
            return users.GetAllUsersIds();
        }

        //returns a list of all the items in the dataset
        public List<string> GetAllItems()
        {
            return items.GetAllItemsIds();
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

        public Dictionary<double, int> GetRatingsHistogram(string sUID)
        {
            throw new NotImplementedException();
        }

        //predict a rating for a user item pair using the specified method
        public double PredictRating(PredictionMethod m, string sUID, string sIID)
        {
            User user = users.getUserById(sUID);
            Item item = items.GetItemById(sIID);
            return predictionEngine.Predict(m, user, item);
        }

        //Compute MAE (mean absolute error) for a set of rating prediction methods over the same user-item pairs
        //cTrials specifies the number of user-item pairs to be tested
        public Dictionary<PredictionMethod, double> ComputeMAE(List<PredictionMethod> lMethods, int cTrials)
        {
            List<KeyValuePair<User, string>> userItemTestSet = evaluationEngine.createTestSet(cTrials, testUsers);
            return evaluationEngine.ComputeMAE(predictionEngine,lMethods, userItemTestSet); 
        }

        public Dictionary<PredictionMethod, double> ComputeRMSE(List<PredictionMethod> lMethods, int cTrials = 0)
        {
            Console.WriteLine("*****************    Model Evaluation    *********************");
            Dictionary<RecommenderSystem.PredictionMethod, double> results = new Dictionary<PredictionMethod, double>();

            foreach (var method in lMethods)
            {
                IPredictionModel model = predictionEngine.getModel(method);
                if (model != null)
                {
                    var rmse = evaluationEngine.computeRMSE(testUsers, testItems, model);
                    Console.WriteLine(String.Format("Model: {0}, RMSE: {1}", method, rmse));
                    results.Add(method, rmse);
                }
            }
            Console.WriteLine("*************************************************************");

            return results;
        }

        public Dictionary<PredictionMethod, double> ComputeRMSE(List<PredictionMethod> lMethods, out Dictionary<PredictionMethod, Dictionary<PredictionMethod, double>> dConfidence)
        {

            // compute RMSE
            Dictionary<PredictionMethod, double> results = new Dictionary<PredictionMethod, double>();
            foreach (var method in lMethods)
            {
                IPredictionModel model = predictionEngine.getModel(method);
                if (model != null)
                {
                    var rmse = evaluationEngine.computeRMSE(testUsers, testItems, model);
                    results.Add(method, rmse);
                }
            }       

            // compute dConfidence
            dConfidence = new Dictionary<PredictionMethod, Dictionary<PredictionMethod, double>>();
            foreach (var method in lMethods)
                dConfidence.Add(method, new Dictionary<PredictionMethod, double>());


            List<Tuple<PredictionMethod, PredictionMethod>> methodPairs = DataUtils.getAllPairedCombinations(lMethods);
            foreach (var methodPair in methodPairs)
            {
                PredictionMethod method1 = methodPair.Item1;
                PredictionMethod method2 = methodPair.Item2;

                Tuple<double, double> pApB = evaluationEngine.computeConfidence(testUsers, testItems, predictionEngine.getModel(method1), predictionEngine.getModel(method2));
                dConfidence[method1].Add(method2, pApB.Item1);
                dConfidence[method2].Add(method1, pApB.Item2);
            }


            return results;
        }

        public List<string> Recommend(RecommendationMethod sAlgorithm, string sUserId, int cRecommendations)
        {
            List<string> result = new List<string>();

            switch (sAlgorithm)
            {
                case (RecommendationMethod.Popularity):
                    result = GetPopularItems(sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.Pearson):
                    result = GetTopItems(predictionEngine.getModel(PredictionMethod.Pearson), sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.Cosine):
                    result = GetTopItems(predictionEngine.getModel(PredictionMethod.Cosine), sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.BaseModel):
                    result = GetTopItems(predictionEngine.getModel(PredictionMethod.BaseModel), sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.Stereotypes):
                    result = GetTopItems(predictionEngine.getModel(PredictionMethod.Stereotypes), sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.CP):
                    result = GetTopCpItems(sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.NNPearson):
                    result = GetTopItemsBasedNN(new PearsonMethod(), sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.NNCosine):
                    result = GetTopItemsBasedNN(new CosineMethod(), sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.NNBaseModel):
                    break;
            }

            return result;
        }

        Dictionary<int, Dictionary<RecommendationMethod, Dictionary<string, double>>> ComputePrecisionRecall(List<RecommendationMethod> lMethods, List<int> lLengths, int cTrials)
        {
            throw new NotImplementedException();
        }

        public void CalculateAverageRatingForTrainingSet()
        {
            double sum = trainUsers.Sum(user => user.GetAverageRatings());

            this.averageTrainRating = sum / trainUsers.Count();
        }

        #region private methods

        private double getAverageRating(Users userSet)
        {
           double sum = userSet.Sum(user => user.GetAverageRatings());

           return sum / userSet.Count();
        }

        private List<string> GetPopularItems(string sUserId, int cRecommendations)
        {
            // take only items that has not rated by the user and order them by popularity 
            return items.Where(item => !item.GetRatingUsers().Contains(sUserId)).OrderByDescending(item => item.GetRatingUsers().Count()).Select(item => item.GetId()).Take(cRecommendations).ToList();
        }

        private List<string> GetTopItems(IPredictionModel predictionModel, string sUserId, int cRecommendations)
        {
            var currentUser = users.getUserById(sUserId);
            
            //TODO need to think about scenario of new user (without any rated items)
            var candidateItems = items.Where(item => !item.GetRatingUsers().Contains(sUserId));
            var orderByPrdiction = candidateItems.OrderByDescending(item => predictionModel.Predict(currentUser, item));
            return orderByPrdiction.Select(item => item.GetId()).Take(cRecommendations).ToList();
        }

        private List<string> GetTopCpItems(string sUserId, int cRecommendations)
        {
            User user = users.getUserById(sUserId);
            List<KeyValuePair<Item, double>> recommendedItems = conditionalProbabilityModel.Recommend(user);
            return recommendedItems.Select(kv => kv.Key.GetId()).ToList();
        }

        private List<string> GetTopItemsBasedNN(ISimilarityMethod similarityMethod, string sUserId, int cRecommendations)
        {
            List<string> result = new List<string>();

            User currentUser = users.getUserById(sUserId);
            var candidateItems = items.Where(item => !item.GetRatingUsers().Contains(sUserId));
            var NNList = users.Where(user => !user.Equals(currentUser)).OrderByDescending(user => similarityEngine.calculateSimilarity(similarityMethod, currentUser, user)).Take(20); //TODO Set K
            
            // Eyal Need to start from here 
            return result;
        }

        #endregion
    }
}
