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
        public enum RecommendationMeasure { Precision, Recall };

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
        private ItemBasedEngine itemBasedEngine; 

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

        public HashSet<string> GetTestUsers(){
            return new HashSet<string>(this.testUsers.Select(user => user.GetId()));
        }

        public HashSet<string> GetTestUserItems(string sUserId)
        {
            return new HashSet<string>(this.testUsers.getUserById(sUserId).GetRatedItems());
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

            similarityEngine = new SimilarityEngine(trainUsers, MAX_SIMILAR_USERS, logger);  //TODO - check whether it should be train/test users
            evaluationEngine = new EvaluationEngine(averageTrainRating);

            predictionEngine.addModel(PredictionMethod.Cosine, new CollaborativeFilteringModel(trainUsers, trainItems, similarityEngine, new CosineMethod()));
            predictionEngine.addModel(PredictionMethod.Pearson, new CollaborativeFilteringModel(trainUsers, trainItems, similarityEngine, new PearsonMethod()));
            predictionEngine.addModel(PredictionMethod.Random, new CollaborativeFilteringModel(trainUsers, trainItems, similarityEngine, new RandomMethod()));

            itemBasedEngine = new ItemBasedEngine(trainItems); //TODO - check that we need to use test items...
            itemBasedEngine.calculateIntersectionInBackground();
        }

        public string getTestUserId()
        {
            RandomGenerator randomGenerator = new RandomGenerator();
            return randomGenerator.getRandomUser(testUsers).GetId();
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
                case (RecommendationMethod.NNPearson):
                    result = GetTopItemsBasedNN(new PearsonMethod(), sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.NNCosine):
                    result = GetTopItemsBasedNN(new CosineMethod(), sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.NNBaseModel):
                    result = GetTopItemsBasedNN(new BaseModelMethod(predictionEngine.getModel(PredictionMethod.BaseModel)), sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.NNJaccard):
                    result = GetTopItemsBasedNN(new JaccardMethod(), sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.CP):
                    result = GetTopItemsCp(sUserId, cRecommendations);
                    break;
                case (RecommendationMethod.Jaccard):
                    result = GetTopItemsJaccard(sUserId, cRecommendations);
                    break;
            }

            return result;
        }

        public Dictionary<int, Dictionary<string, Dictionary<RecommendationMethod, double>>> ComputePrecisionRecall(List<RecommendationMethod> lMethods, List<int> lLengths, int cTrials)
        {
            string precisionString = RecommendationMeasure.Precision.ToString();
            string recallString = RecommendationMeasure.Recall.ToString();

            Dictionary<int, Dictionary<string, Dictionary<RecommendationMethod, double>>> ans = new Dictionary<int, Dictionary<string, Dictionary<RecommendationMethod, double>>>();
            int maxLength = lLengths.Max();

            //intialize values
            foreach (var len in lLengths)
            {
                ans.Add(len, new Dictionary<string, Dictionary<RecommendationMethod, double>>());
                ans[len].Add(precisionString, new Dictionary<RecommendationMethod, double>());
                ans[len].Add(recallString, new Dictionary<RecommendationMethod, double>());
                foreach (var method in lMethods)
                {
                    ans[len][precisionString].Add(method, 0);
                    ans[len][recallString].Add(method, 0);
                }
            }

            int counterTest = 0;
            //for each test user, get recommened list of size N and calcualte measures: Precision, Recall
            foreach (var user in testUsers)
	        {
                string userId = user.GetId();
                Console.Out.WriteLine("userId: " + userId);
		        foreach (var method in lMethods)
                {
                    var recommended = Recommend(method, userId, maxLength);
                    Console.Out.WriteLine("   method: " + method.ToString());
                    foreach (var len in lLengths)
                    {
                        var userRatedItems = user.GetRatedItems();
                        double tp = recommended.Take(len).Intersect(userRatedItems).Count(); 
                        double fp = len - tp;
                        double fn = userRatedItems.Count - tp;

                        double precision = tp / (tp + fp);
                        double recall = tp / (tp + fn);

                        ans[len][precisionString][method] += precision;
                        ans[len][recallString][method] += recall;
                    }
                }
                counterTest++;
	        }

            //for each size of list, calculate the precision and recall
            foreach (var len in lLengths)
            {
                foreach (var method in lMethods)
                {
                    ans[len][precisionString][method] = ans[len][precisionString][method] / testUsers.Count();
                    ans[len][recallString][method] = ans[len][recallString][method] / testUsers.Count();
                }
            }

            return ans;
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
            Dictionary<string, int> result = new Dictionary<string, int>();

            // take only items that has not rated by the user and order them by popularity 
            foreach (var item in trainItems)
            {
                var ratingUsers = item.GetRatingUsers();
                if (!ratingUsers.Contains(sUserId))
                {
                    result.Add(item.GetId(), ratingUsers.Count);
                }
            }

            var orderedList = result.OrderByDescending(item => item.Value);
            return orderedList.Select(item => item.Key).Take(cRecommendations).ToList();
        }

        private List<string> GetTopItems(IPredictionModel predictionModel, string sUserId, int cRecommendations)
        {
            var currentUser = testUsers.getUserById(sUserId);

            var currentItems = trainUsers.getUserById(sUserId).GetRatedItems(); //in case the user is also in the training set we want to filter out those rated items from train set
            var candidateItems = trainItems.GetAllItemsIds().Except(currentItems); //select items that current user is not yet rated

            var candidateItemsDic = candidateItems.ToDictionary(item => item, item => predictionModel.Predict(currentUser, trainItems.GetItemById(item)));
            var orderByPrediction = candidateItemsDic.OrderByDescending(item => item.Value);

            return orderByPrediction.Select(item => item.Key).Take(cRecommendations).ToList();
        }

        private List<string> GetTopItemsBasedNN(ISimilarityMethod similarityMethod, string sUserId, int cRecommendations)
        {
            Dictionary<string, double> itemScore = new Dictionary<string, double>();
            int k = 20; //number of NN

            //Select an item only if one of the neighbors has rated it
            User currentUser = testUsers.getUserById(sUserId);
            var NNList = trainUsers.Where(user => !user.Equals(currentUser));
            var NNListDic = NNList.ToDictionary(user => user, user => similarityEngine.calculateSimilarity(similarityMethod, currentUser, user));
            var NNListOrderedDic = NNListDic.OrderByDescending(user => user.Value);
            var NNTopK = NNListOrderedDic.Take(k);

            //For each item that rated by one of the neighbors, calculate the normalized rating score
            foreach (var user in NNTopK)
            {
                double weight = user.Value;
                var itemList = user.Key.GetRatedItems();
                foreach (var item in itemList)
                {
                    if (!itemScore.ContainsKey(item))
                    {
                        itemScore.Add(item, weight);
                    }
                    else
                    {
                        itemScore[item] += weight;
                    }
                }

            }

            var orderedItemScore = itemScore.OrderByDescending(item => item.Value);
            return orderedItemScore.Select(item => item.Key).Take(cRecommendations).ToList(); ;
        }

        private List<string> GetTopItemsCp(string sUserId, int cRecommendations)
        {
            List<Item> givenItems = trainUsers
                                    .getUserById(sUserId)
                                    .GetRatedItems()
                                    .Select(itemId => trainItems.GetItemById(itemId))
                                    .ToList();

            return itemBasedEngine
                .getConditionalProbability(givenItems)
                .Select(kv => kv.Key.GetId())
                .ToList()
                .GetRange(0,cRecommendations);
        }

        private List<string> GetTopItemsJaccard(string sUserId, int cRecommendations)
        {
            List<Item> givenItems = trainUsers
                        .getUserById(sUserId)
                        .GetRatedItems()
                        .Select(itemId => trainItems.GetItemById(itemId))
                        .ToList();

            return itemBasedEngine
                .getJaccardProbability(givenItems)
                .Select(kv => kv.Key.GetId())
                .ToList()
                .GetRange(0, cRecommendations);
        }

        #endregion
    }
}
