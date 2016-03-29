using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace RecommenderSystem
{
    [TestClass]
    public class BoundedSortedCollectionTest
    {
        [TestMethod]
        public void testAdd()
        {
            BoundedSortedCollection<int> collection = new BoundedSortedCollection<int>(2);
            collection.add(2);
            collection.add(1);
            collection.add(0);
            collection.add(3);

            List<int> actual = collection.AsList();
            List<int> expected = new List<int>(){2,3};
            
            CollectionAssert.AreEqual(expected,actual);
      }
    }
}
