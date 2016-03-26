using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assignment1;
using System.Collections;
using System.Collections.Generic;

namespace Assignment1Test.Engines
{
    [TestClass]
    public class SimilarityEngineTest
    {
        [TestMethod]
        public void testcalCulateSimilarity()
        {
            ILogger logger = new DebugLogger();

            User user = new User("user");
            User differentUserValues = new User("differentUserValues");
            User sameUserValues = new User("sameUserValues");

            user.AddItem("item1", 3);
            user.AddItem("item2", 7);
            user.AddItem("item3", 6);
            user.AddItem("item4", 4);
            user.AddItem("item5", 1);
            user.AddItem("item6", 1);
            user.AddItem("item7", 5);

            sameUserValues.AddItem("item1", 3);
            sameUserValues.AddItem("item2", 7);
            sameUserValues.AddItem("item3", 6);
            sameUserValues.AddItem("item4", 4);
            sameUserValues.AddItem("item5", 1);
            sameUserValues.AddItem("item6", 1);
            sameUserValues.AddItem("item7", 5);

            differentUserValues.AddItem("item1", 6);
            differentUserValues.AddItem("item2", 4);
            differentUserValues.AddItem("item3", 3);
            differentUserValues.AddItem("item4", 7);
            differentUserValues.AddItem("item5", 6);
            differentUserValues.AddItem("item6", 4);
            differentUserValues.AddItem("item7", 1);


            Users users = new Users();
            users.addUser(user);
            users.addUser(differentUserValues);
            users.addUser(sameUserValues);
            
            SimilarityEngine similarityEngine = new SimilarityEngine(users,1,logger);
            List<KeyValuePair<double, User>> actual = similarityEngine.calculateSimilarity(new PearsonMethod(), user);
            List<KeyValuePair<double, User>> expected = new List<KeyValuePair<double, User>>(){
                new KeyValuePair<double, User>(1.0,sameUserValues)
            };
            CollectionAssert.AreEqual(expected, actual);

            
        }
    }
}

