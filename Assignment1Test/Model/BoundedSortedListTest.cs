using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assignment1;
using System.Collections;
using System.Collections.Generic;

namespace Assignment1Test
{
    [TestClass]
    public class BoundedSortedListTest
    {
        [TestMethod]
        public void testAdd()
        {
            BoundedSortedList<double, string> boundedList = new BoundedSortedList<double, string>(2);

            boundedList.add(1.1, "first");
            boundedList.add(1.0, "second");
            boundedList.add(0.6, "third");
            boundedList.add(1.05, "fourth");

            List<KeyValuePair<double, string>> expected = new List<KeyValuePair<double, string>>(){
                new KeyValuePair<double,string>(1.05,"fourth"),
                new KeyValuePair<double,string>(1.1,"first"),
            };

            List<KeyValuePair<double, string>> actual = boundedList.getList();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
