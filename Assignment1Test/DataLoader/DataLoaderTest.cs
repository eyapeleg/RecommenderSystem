using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecommenderSystem;

namespace Assignment1Test.DataLoader
{
    [TestClass]
    public class DataLoaderTest
    {
       /* [TestMethod]
        public void TestDataLoadWithTrainSize()
        {
            double dTrainSize = 0.95;
            string fileName = "ratings.dat";

            ILogger logger = new DebugLogger();
            DataLoaderEngine dl = new DataLoaderEngine(logger);

            Dictionary<IDatasetType, Tuple<Users, Items>> results = dl.Load(fileName, dTrainSize);
            int dSize = dl.GetDataSetSize(fileName);

            int expected = (int)(dSize * (1 - dTrainSize));
            Tuple<Users, Items> testData = results["test"];

            int actual = 0;
            for (int i = 0; i < testData.Item1.Count(); i++)
            {
                var items = testData.Item1.ElementAt(i).GetRatedItems();
                for (int j = 0; j < items.Count; j++)
                {
                    actual++;
                }
            }

            Assert.AreEqual(expected, actual);
        }*/
    }
}
