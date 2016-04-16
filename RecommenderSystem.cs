using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace RecommenderSystem
{
    public class RecommenderSystem
    {
        public enum PredictionMethod { Pearson, Cosine, Random, BaseModel, Stereotypes };
        public enum DatasetType { Train, Test, Validation};
        PredictionMethodsMapping predictionMethodsMapping;
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
        private MatrixFactorizationEngine matrixFactorizationEngine;

        private ILogger logger;

        private int MAX_SIMILAR_USERS;
        private int NUM_OF_TRIALS;
        private int dsSize;

        public RecommenderSystem()
        {
            logger = new InfoLogger();
            dataLoaderEngine = new DataLoaderEngine(logger);

            MAX_SIMILAR_USERS = 30;
            NUM_OF_TRIALS = 1000;
            predictionMethodsMapping = new PredictionMethodsMapping();           
        }

        public void Load(string sFileName)
        {
            Tuple<Users, Items> data = dataLoaderEngine.Load(sFileName);
            users = data.Item1;
            items = data.Item2;
            similarityEngine = new SimilarityEngine(users, MAX_SIMILAR_USERS, logger);
            predictionEngine = new PredictionEngine(users, items, predictionMethodsMapping, similarityEngine);
            evaluationEngine = new EvaluationEngine(users);
        }

        public void Load(string sFileName, double dTrainSetSize)
        {
            Tuple<Users, Items> data = dataLoaderEngine.Load(sFileName);
            Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>> splittedData;
            
            this.dsSize = dataLoaderEngine.GetDataSetSize();
            splittedData  = dataUtils.Split(dTrainSetSize, dsSize, data, DatasetType.Test, DatasetType.Train);

            trainUsers = splittedData[DatasetType.Train].Item1;
            trainItems = splittedData[DatasetType.Train].Item2;
            testUsers = splittedData[DatasetType.Test].Item1;
            testItems = splittedData[DatasetType.Test].Item2;

            double trainSize = Math.Round(dsSize * 0.95); //TODO - parameterize hardcoded data size
            splittedData = dataUtils.Split(0.95, trainSize, new Tuple<Users, Items>(this.trainUsers, this.trainItems), DatasetType.Validation, DatasetType.Train);//TODO - parameterize hardcoded data size

            trainUsers = splittedData[DatasetType.Train].Item1;
            trainItems = splittedData[DatasetType.Train].Item2;
            validationUsers = splittedData[DatasetType.Validation].Item1;
            validationItems = splittedData[DatasetType.Validation].Item2;

            //calculate the overall average rating 
            CalculateAverageRatingForTrainingSet();

            similarityEngine = new SimilarityEngine(trainUsers, MAX_SIMILAR_USERS, logger);
        }

        public void TrainBaseModel(int cFeatures)
        {
            MatrixFactorizationEngine matrixFactorizationEngine = new MatrixFactorizationEngine(trainUsers, trainItems, validationUsers, validationItems);
            matrixFactorizationEngine.train(cFeatures, averageTrainRating); //TODO - modify the average rating to be only on the small train set
        }

        public void TrainStereotypes(int cStereotypes)
        {
            StereotypesEngine engine = new StereotypesEngine(similarityEngine, new PearsonMethod());
            Stereotypes stereotypes = engine.initStereotypes(trainUsers, trainItems, cStereotypes);
            engine.trainStereotypes(stereotypes, trainUsers);

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
            return predictionEngine.PredictRating(m, user, item);
        }

        //Compute MAE (mean absolute error) for a set of rating prediction methods over the same user-item pairs
        //cTrials specifies the number of user-item pairs to be tested
        public Dictionary<PredictionMethod, double> ComputeMAE(List<PredictionMethod> lMethods, int cTrials)
        {
            List<KeyValuePair<User, string>> userItemTestSet = evaluationEngine.createTestSet(cTrials);
            return evaluationEngine.ComputeMAE(predictionEngine,lMethods, userItemTestSet); //TODO - verify that prediction engine is not null
        }

        public Dictionary<PredictionMethod, double> ComputeRMSE(List<PredictionMethod> lMethods, int cTrials)
        {
            throw new NotImplementedException();
        }

        public Dictionary<PredictionMethod, double> ComputeRMSE(List<PredictionMethod> lMethods, out Dictionary<PredictionMethod, Dictionary<PredictionMethod, double>> dConfidence)
        {
            throw new NotImplementedException();
        }

        public void CalculateAverageRatingForTrainingSet()
        {
            double sum = trainUsers.Sum(user => user.GetAverageRatings());

            this.averageTrainRating = sum / trainUsers.Count();
        }
    }
}
