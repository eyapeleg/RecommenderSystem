using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RecommenderSystem
{
    public class DataLoaderEngine
    {
        ILogger logger;

        public DataLoaderEngine(ILogger logger)
        {
            this.logger = logger;
        }

        public Tuple<Users,Items> Load(string sFileName)
        {

            Users users = new Users();
            Items items = new Items();

            logger.info("Starting load data...");
            //TODO: Remove stopwatch on submittion 
            Stopwatch timer = Stopwatch.StartNew();

            StreamReader objInput = new StreamReader(sFileName, Encoding.Default);
            while (!objInput.EndOfStream)
            {
                string line = objInput.ReadLine();
                string[] split = System.Text.RegularExpressions.Regex.Split(line, "::", RegexOptions.None);
                string userId = split[0];
                string itemId = split[1];
                double rating = Convert.ToDouble(split[2]);

                logger.debug("read user"+" ["+userId+"]"+" itemId"+" ["+itemId+"]"+" rating"+" ["+rating+"]");
                users.addItemToUser(userId, itemId, rating);
                items.addUserToItems(userId, itemId, rating);
            }

            //initialize data for each predict method 
            //pearson.calcAverageRatingPerUser();

            timer.Stop();
            TimeSpan elapsed = timer.Elapsed;
            logger.info("Loading data was completed successfully\nExection Time: "+elapsed.ToString("mm':'ss':'fff"));

            return Tuple.Create<Users, Items>(users, items);
        }

        public Dictionary<string, Tuple<Users, Items>> Load(string sFileName, double dTrainSetSize)
        {
            Dictionary<string, Tuple<Users, Items>> result = new Dictionary<string, Tuple<Users, Items>>();
            Random rnd = new Random();
            var trainUsers = new Users();
            var testUsers = new Users();
            var trainItems = new Items();
            var testItems = new Items();

            logger.info("Starting load data - train/test split...");
            Stopwatch timer = Stopwatch.StartNew();

            StreamReader objInput = new StreamReader(sFileName, Encoding.Default);
            int linesCount = File.ReadAllLines(sFileName).Count();
            int testSize = (int)  (linesCount * (1 - dTrainSetSize));
            int count = 0;

            while (!objInput.EndOfStream)
            {
                var nextDouble = rnd.NextDouble();

                string line = objInput.ReadLine();
                string[] split = Regex.Split(line, "::", RegexOptions.None);
                string userId = split[0];
                string itemId = split[1];
                double rating = Convert.ToDouble(split[2]);

                logger.debug("read user" + " [" + userId + "]" + " itemId" + " [" + itemId + "]" + " rating" + " [" + rating + "]");

                //with probablity of 0.5, assign the user to train/test set
                if (nextDouble <= 0.5 && count < testSize)
                {
                    if (!result.ContainsKey("test"))
                    {
                        testUsers.addItemToUser(userId, itemId, rating);
                        testItems.addUserToItems(userId, itemId, rating);
                        result.Add("test", new Tuple<Users, Items>(testUsers, testItems));
                    }
                    else
                    {
                        result["test"].Item1.addItemToUser(userId, itemId, rating);
                        result["test"].Item2.addUserToItems(userId, itemId, rating);
                    }
                    count++;
                }
                else
                {
                    if (!result.ContainsKey("train"))
                    {
                        trainUsers.addItemToUser(userId, itemId, rating);
                        trainItems.addUserToItems(userId, itemId, rating);
                        result.Add("train", new Tuple<Users, Items>(trainUsers, trainItems));
                    }
                    else
                    {
                        result["train"].Item1.addItemToUser(userId, itemId, rating);
                        result["train"].Item2.addUserToItems(userId, itemId, rating);
                    }
                }
            }
            return result;
        }

        public int GetDataSetSize(string sFileName)
        {
            StreamReader objInput = new StreamReader(sFileName, Encoding.Default);
            int linesCount = File.ReadAllLines(sFileName).Count();
            return linesCount;
        }


    }
}
