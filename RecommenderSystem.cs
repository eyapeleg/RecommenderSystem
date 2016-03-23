using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RecommenderSystem
{
    class RecommenderSystem
    {
        public enum PredictionMethod { Pearson, Cosine, Random };
        public Dictionary<string, Dictionary<string, double>> userData;
        public Dictionary<string, Dictionary<string, double>> itemData;

        //class members here

        //constructor
        public RecommenderSystem()
        {
            userData = new Dictionary<string, Dictionary<string, double>>();
            itemData = new Dictionary<string, Dictionary<string, double>>();
        }

        //load a datatset 
        //The file contains one row for each u,i rating, in the following format:
        //userid::itemid::rating::timestamp
        //More at http://recsyswiki.com/wiki/Movietweetings
        //Download at https://github.com/sidooms/MovieTweetings/tree/master/latest
        //Do all precomputations here if needed
        public void Load(string sFileName)
        {
            Console.WriteLine("Starting load data...");
            //TODO: Remove stopwatch on submittion 
            Stopwatch timer = Stopwatch.StartNew();

            StreamReader objInput = new StreamReader(sFileName, System.Text.Encoding.Default);
            while (!objInput.EndOfStream)
            {
                string line = objInput.ReadLine();
                string[] split = System.Text.RegularExpressions.Regex.Split(line, "::", RegexOptions.None);
                string userId = split[0];
                string itemId = split[1];
                double rating = Convert.ToDouble(split[2]);
                InsertValuesToDictionary(userId, itemId, rating);
            }

            PearsonMethod pm = new PearsonMethod();
            pm.calcAverageRatingPerUser(userData);

            timer.Stop();
            TimeSpan elapsed = timer.Elapsed;
            Console.WriteLine("Loading data was completed successfully\nExection Time: {0}\n", elapsed.ToString("mm':'ss':'fff"));
        }

        //return a list of the ids of all the users in the dataset
        public List<string> GetAllUsers()
        {
            return userData.Keys.ToList();
        }

        //returns a list of all the items in the dataset
        public List<string> GetAllItems()
        {
            return itemData.Keys.ToList();
        }

        //returns the list of all items that the given user has rated in the dataset
        public List<string> GetRatedItems(string sUID)
        {
            Dictionary<string, double> items;

            if(userData.TryGetValue(sUID, out items))
            {
                return items.Keys.ToList();
            }

            return null;
        }

        //Returns a user-item rating that appears in the dataset (not predicted)
        public double GetRating(string sUID, string sIID)
        {
            Dictionary<string, double> items;
            double rating; 

            if(userData.TryGetValue(sUID, out items) && items.TryGetValue(sIID, out rating))
            {
                return rating;
            }
            return 0;
        }

        //predict a rating for a user item pair using the specified method
        public double PredictRating(PredictionMethod m, string sUID, string sIID)
        {
            throw new NotImplementedException();
        }

        //Compute MAE (mean absolute error) for a set of rating prediction methods over the same user-item pairs
        //cTrials specifies the number of user-item pairs to be tested
        public Dictionary<PredictionMethod, double> ComputeMAE(List<PredictionMethod> lMethods, int cTrials)
        {
            throw new NotImplementedException();
        }

        #region private methods
        private void InsertValuesToDictionary(string userId, string itemId, double rating)
        {
            //insert values to user data dictionary
            if (userData.ContainsKey(userId))
            {
                userData[userId].Add(itemId, rating);
            }
            else
            {
                var value = new Dictionary<string, double>() { { itemId, rating } };
                userData.Add(userId, value);
            }

            //insert values to item data dictionary
            if (itemData.ContainsKey(itemId))
            {
                itemData[itemId].Add(userId, rating);
            }
            else
            {
                var value = new Dictionary<string, double>() { { userId, rating } };
                itemData.Add(itemId, value);
            }
        }
        #endregion
    }
}
