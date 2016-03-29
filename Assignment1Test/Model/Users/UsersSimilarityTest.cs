using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RecommenderSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assignment1Test
{
    [TestClass]
    public class BoundedSortedListTest
    {
        [TestMethod]
        public void testAdd()
        {
            UsersSimilarity usersSimilarity = new UsersSimilarity(2);

            usersSimilarity.add(new User("first"), 1.1);
            usersSimilarity.add(new User("second"),1.0);
            usersSimilarity.add(new User("third"),0.6);
            usersSimilarity.add(new User("fourth"),1.05);

            List<KeyValuePair<User, double>> expected = new List<KeyValuePair<User, double>>(){
                new KeyValuePair<User, double>(new User("fourth"),1.05),
                new KeyValuePair<User, double>(new User("first"),1.1)
            };

            List<KeyValuePair<User, double>> actual = usersSimilarity.AsList();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void testAddDuplicateValues()
        {
             UsersSimilarity usersSimilarity = new UsersSimilarity(2);

            usersSimilarity.add(new User("first"), 1.1);
            usersSimilarity.add(new User("second"), 1.1);

            List<KeyValuePair<User, double>> expected = new List<KeyValuePair<User, double>>(){
                new KeyValuePair<User, double>(new User("first"), 1.1),
                new KeyValuePair<User, double>(new User("second"), 1.1)
            };


            List<KeyValuePair<User, double>> actual = usersSimilarity.AsList();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
