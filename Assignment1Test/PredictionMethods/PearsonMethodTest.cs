﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecommenderSystem;
namespace Assignment1Test
{
    [TestClass]
    public class PearsonMethodTest
    {
        [TestMethod]
        public void TestCalculateSimilarity()
        {
            User user1 = new User("user1");
            User user2 = new User("user2");

            user1.AddItemById("item1", 3);
            user1.AddItemById("item2", 7);
            user1.AddItemById("item3", 6);
            user1.AddItemById("item4", 4);
            user1.AddItemById("item5", 1);
            user1.AddItemById("item6", 1);
            user1.AddItemById("item7", 5);

            user2.AddItemById("item1", 6);
            user2.AddItemById("item2", 4);
            user2.AddItemById("item3", 3);
            user2.AddItemById("item4", 7);
            user2.AddItemById("item5", 6);
            user2.AddItemById("item6", 4);
            user2.AddItemById("item7", 1);

            PearsonMethod PearsonMethod = new PearsonMethod();
            string expetected = String.Format("{0:0.00}",-0.4325);
            List<string> intersectList = user1.GetRatedItems().Intersect(user2.GetRatedItems()).ToList();
            string acutal = String.Format("{0:0.00}",PearsonMethod.calculateSimilarity(user1, user2,intersectList));
            Assert.AreEqual(expetected,acutal);
        }
    }
}
