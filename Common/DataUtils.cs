using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RecommenderSystem
{
    public class DataUtils
    {
        static ILogger logger = new InfoLogger();
        RandomGenerator randomGenerator = new RandomGenerator();

        public Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>> Split(double subsetSize, double datasetSize, 
            Tuple<Users, Items> data, RecommenderSystem.DatasetType smallSetType, RecommenderSystem.DatasetType largeSetType)
        {
            logger.info("Start data split...");
            Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>> result = new Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>>();
            Stopwatch timer = Stopwatch.StartNew();

            //initialize the values of desired and current test size
            double smallDsSize = Math.Round(datasetSize* (1- subsetSize));
            int currentTestSize = 0;

            //get the list of users
            Users users = data.Item1;

            //initialize test/train sets
            result.Add(largeSetType, new Tuple<Users, Items>(new Users(), new Items()));
            result.Add(smallSetType, new Tuple<Users, Items>(new Users(), new Items()));

            do
            {
                //select random user
                User user = randomGenerator.newRandomUser(users);
                var numberOfRatings = user.GetRatedItems().Count;
                string userId = user.GetId();

                //verify the user has at least 2 rated Items
                if (numberOfRatings > 2)
                {
                    List<string> smallSetItems = randomGenerator.newRandomItemsPerUser(user);
                    List<string> largeSetItems = user.GetRatedItems().Except(smallSetItems).ToList();
                    currentTestSize += smallSetItems.Count;

                    AddUserToDataset(result, user, smallSetType, smallSetItems); //random items goes to small set
                    AddUserToDataset(result, user, largeSetType, largeSetItems); //the rest of the items goes to large set
                }
                else //add user with less than two items to the large dataset
                {
                    AddUserToDataset(result, user, largeSetType, user.GetRatedItems());
                }

                //remove the user from the full list once the 
                users.RemoveUserById(userId);

            } while (currentTestSize < smallDsSize);

            //Add the rest of the users to the large dataset
            foreach (var user in data.Item1)
            {
                AddUserToDataset(result, user, largeSetType, user.GetRatedItems());
            }

            timer.Stop();
            TimeSpan elapsed = timer.Elapsed;
            logger.info(String.Format("Split data to {0}/{1} was completed successfully\nExection Time: {2}" , largeSetType, smallSetType, elapsed.ToString("mm':'ss':'fff")));

            return result;
        }

        private void AddUserToDataset(Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>> result, 
                                        User user, RecommenderSystem.DatasetType dsType, List<string> items)
        {
            string userId = user.GetId();

            foreach (var itemId in items)
            {
                double rating = user.GetRating(itemId);
                result[dsType].Item1.addItemToUser(userId, itemId, rating);
                result[dsType].Item2.addUserToItem(userId, itemId, rating);
            }
        }
    }


}
