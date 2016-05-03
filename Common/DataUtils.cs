using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace RecommenderSystem
{
    public class DataUtils
    {
        static ILogger logger = new InfoLogger();
        RandomGenerator randomGenerator = new RandomGenerator();
        double minRatingThreshold = 10; //Double.Parse(ConfigurationManager.AppSettings["split_MinimunRatedItemsPerUser"]); 

        public Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>> Split(double subsetSize, double datasetSize, 
            Tuple<Users, Items> data, RecommenderSystem.DatasetType smallSetType, RecommenderSystem.DatasetType largeSetType)
        {
            logger.info("Start data split...");
            Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>> result = new Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>>();
            Stopwatch timer = Stopwatch.StartNew();

            //initialize the values of desired and current test size
            double smallDsSize = Math.Round(datasetSize* (1- subsetSize));
            int currentTestSize = 0;

            //initialize test/train sets
            result.Add(largeSetType, data);
            result.Add(smallSetType, new Tuple<Users, Items>(new Users(), new Items()));
            var subsetUsers = result[largeSetType].Item1.Where(x => x.GetRatedItems().Count >= minRatingThreshold);

            do
            {
                //select random user
                User user = randomGenerator.getRandomUser(subsetUsers);
                var numberOfRatings = user.GetRatedItems().Count;

                Dictionary<string, double> smallSetItems = randomGenerator.NewRandomItemsPerUser(user);
                currentTestSize += smallSetItems.Count;

                AddUserToSmallDataset(result[smallSetType], user, smallSetItems); //random items goes to small set
                RemoveUserFromLargeDataset(user, smallSetItems); //the rest of the items goes to large set

            } while (currentTestSize < smallDsSize);

            timer.Stop();
            TimeSpan elapsed = timer.Elapsed;
            logger.info(String.Format("Split data to {0}/{1} was completed successfully\nExection Time: {2}" , largeSetType, smallSetType, elapsed.ToString("mm':'ss':'fff")));

            return result;
        }

        private void RemoveUserFromLargeDataset(User user, Dictionary<string, double> smallSetItems)
        {
            foreach (var item in smallSetItems)
            {
                user.RemoveItemById(item.Key);
            }
        }

        private void AddUserToSmallDataset(Tuple<Users, Items> result, User user, Dictionary<string, double> items)
        {
            string userId = user.GetId();

            foreach (var element in items)
            {
                string itemId = element.Key;
                double rating = element.Value;
                result.Item1.addItemToUser(userId, itemId, rating);
                result.Item2.addUserToItem(userId, itemId, rating);
            }
        }

        public static List<Tuple<E, E>> getAllPairedCombinations<E>(IEnumerable<E> collection)
        {
            List<Tuple<E, E>> result = new List<Tuple<E, E>>();
            for (int i = 0; i < collection.Count(); i++)
                for (int j = i + 1; j < collection.Count(); j++)
                    result.Add(Tuple.Create(collection.ElementAt(i), collection.ElementAt(j)));

            return result;
        }
    }


}
