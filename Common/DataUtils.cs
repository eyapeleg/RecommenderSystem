using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public static class DataUtils
    {
        static ILogger logger = new InfoLogger();

        public static Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>> DataSplit(double subsetSize, double datasetSize, Tuple<Users, Items> data, RecommenderSystem.DatasetType smallSet, RecommenderSystem.DatasetType largeSet)
        {

            Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>> result = new Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>>();

            logger.info("Start data split...");
            Stopwatch timer = Stopwatch.StartNew();

            Random rnd = new Random();

            //initialize the values of desired and current test size
            double smallDsSize = Math.Round(datasetSize* (1- subsetSize));
            int currentTestSize = 0;

            //set the number of total users
            Users users = data.Item1;

            //initialize test/train sets
            result.Add(largeSet, new Tuple<Users, Items>(new Users(), new Items()));
            result.Add(smallSet, new Tuple<Users, Items>(new Users(), new Items()));

            do
            {
                //select random user
                var next = rnd.Next(users.Count() - 1);
                var user = data.Item1.ElementAt(next);
                var numberOfRatings = user.GetRatedItems().Count;
                string userId = user.GetId();

                //verify the user has at least 2 rated Items
                if (numberOfRatings > 2)
                {
                    int kItems = rnd.Next(1, numberOfRatings); //pick a random items from the user rating list
                    int count = 0; //set the number of selected items per user

                    while (count < kItems)
                    {
                        int idx = rnd.Next(numberOfRatings - 1);
                        string itemId = user.GetRatedItems().ElementAt(idx);
                        double rating = user.GetRating(itemId);
                        result[smallSet].Item1.addItemToUser(userId, itemId, rating);
                        result[smallSet].Item2.addUserToItem(userId, itemId, rating);

                        currentTestSize++;
                        user.RemoveItemById(itemId);
                        count++;
                        numberOfRatings--;
                    }

                    foreach (var item in user.GetRatedItems())
                    {
                        double rating = user.GetRating(item);
                        result[largeSet].Item1.addItemToUser(userId, item, rating);
                        result[largeSet].Item2.addUserToItem(userId, item, rating);
                    }
                }
                else //add user with less than two items to the large dataset
                {
                    AddUserToDataset(result, user, largeSet);
                }

                //remove the user from the full list once the 
                users.RemoveUserById(userId);

            } while (currentTestSize < smallDsSize);

            //Add the rest of the users to the large dataset
            foreach (var user in data.Item1)
            {
                AddUserToDataset(result, user, largeSet);
            }

            timer.Stop();
            TimeSpan elapsed = timer.Elapsed;
            logger.info(String.Format("Split data to {0}/{1} was completed successfully\nExection Time: {2}" , largeSet, smallSet, elapsed.ToString("mm':'ss':'fff")));

            return result;
        }

        private static void AddUserToDataset(Dictionary<RecommenderSystem.DatasetType, Tuple<Users, Items>> result, User user, RecommenderSystem.DatasetType dsType)
        {
            string userId = user.GetId();

            foreach (var item in user.GetRatedItems())
            {
                double rating = user.GetRating(item);
                result[dsType].Item1.addItemToUser(userId, item, rating);
                result[dsType].Item2.addUserToItem(userId, item, rating);
            }
        }

    }


}
