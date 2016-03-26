using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RecommenderSystem
{
    class DataLoaderEngine
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

            StreamReader objInput = new StreamReader(sFileName, System.Text.Encoding.Default);
            while (!objInput.EndOfStream)
            {
                string line = objInput.ReadLine();
                string[] split = System.Text.RegularExpressions.Regex.Split(line, "::", RegexOptions.None);
                string userId = split[0];
                string itemId = split[1];
                double rating = Convert.ToDouble(split[2]);

                //logger.debug("read user"+" ["+userId+"]"+" itemId"+" ["+itemId+"]"+" rating"+" ["+rating+"]");
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


    }
}
