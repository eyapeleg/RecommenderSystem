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
            BoundedSortedList<string, double> boundedList = new BoundedSortedList<string, double>(2);

            boundedList.add("first", 1.1);
            boundedList.add("second",1.0);
            boundedList.add("third",0.6);
            boundedList.add("fourth",1.05);

            List<KeyValuePair<double, string>> expected = new List<KeyValuePair<double, string>>(){
                new KeyValuePair<double,string>(1.05,"fourth"),
                new KeyValuePair<double,string>(1.1,"first"),
            };

            List<KeyValuePair<string, double>> actual = boundedList.getList();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void testAddDuplicateValues()
        {
            BoundedSortedList<string, double> boundedList = new BoundedSortedList<string, double>(2);

            boundedList.add("first", 1.1);
            boundedList.add("second", 1.1);

            List<KeyValuePair<double, string>> expected = new List<KeyValuePair<double, string>>(){
                new KeyValuePair<double,string>(1.1,"first"),
                new KeyValuePair<double,string>(1.1,"second")
            };

            List<KeyValuePair<string, double>> actual = boundedList.getList();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
