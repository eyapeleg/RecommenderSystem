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
        private int datasetSize;
        DataUtils dataUtils = new DataUtils();

        public DataLoaderEngine(ILogger logger)
        {
            this.logger = logger;
        }

        public Tuple<Users,Items> Load(string sFileName)
        {
            Users users = new Users();
            Items items = new Items();

            logger.info("Starting load data...");
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
            }

            timer.Stop();
            TimeSpan elapsed = timer.Elapsed;
            logger.info("Loading data was completed successfully\nExection Time: "+elapsed.ToString("mm':'ss':'fff"));

            var blacklistedUsers = users.Where(user => user.GetRatedItems().Count < 5).Select(user => user.GetId()).ToList();
            Console.WriteLine("blacklisted users size: {0}", blacklistedUsers.Count());
            Console.WriteLine("dataset size before removing blacklisted users: {0}", users.Sum(user => user.GetRatedItems().Count()));
            users.removeUsers(blacklistedUsers);

            foreach (var user in users)
            {
                string userId = user.GetId();
                foreach (var item in user.GetItemsRatings())
                {
                    string itemId = item.Key;
                    double rating = item.Value;
                    items.addUserToItem(userId, itemId, rating);
                }
            }

            datasetSize = (int)users.Sum(user => user.GetRatedItems().Count());
            Console.WriteLine("dataset size after removing blacklisted users: {0}", datasetSize);
            return Tuple.Create<Users, Items>(users, items);
        }

        public int GetDataSetSize()
        {
            return datasetSize;
        }


    }
}
