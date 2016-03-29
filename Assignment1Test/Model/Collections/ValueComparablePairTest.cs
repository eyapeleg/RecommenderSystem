using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RecommenderSystem
{
    [TestClass]
    public class ValueComparablePairTest
    {
        [TestMethod]
        public void testEquals()
        {
            ValueComparablePair<string, int> pair = new ValueComparablePair<string, int>("a", 1);
            ValueComparablePair<string, int> pairWithSameKeyAndValue = new ValueComparablePair<string, int>("a", 1);
            Assert.AreEqual(pair, pairWithSameKeyAndValue);
        }

        [TestMethod]
        public void testEqualsSameKeyDiffernetValue()
        {
            ValueComparablePair<string, int> pair = new ValueComparablePair<string, int>("a",2);
            ValueComparablePair<string, int> pairWithSameKeyAndValue = new ValueComparablePair<string, int>("a", 1 );
            Assert.AreEqual(pair, pairWithSameKeyAndValue);
        }

        [TestMethod]
        public void testNotEqualsDifferentKeySameValue()
        {
            ValueComparablePair<string, int> pair = new ValueComparablePair<string, int>("a", 1);
            ValueComparablePair<string, int> pairWithSameKey = new ValueComparablePair<string, int>("b", 1);
            Assert.AreNotEqual(pair, pairWithSameKey);
        }

        [TestMethod]
        public void testCompareToSmaller()
        {
            ValueComparablePair<string, int> pair = new ValueComparablePair<string, int>("a", 2);
            ValueComparablePair<string, int> pairWithSamllerValue = new ValueComparablePair<string, int>("b", 1);
            Assert.AreEqual(pair.CompareTo(pairWithSamllerValue), 1);
        }

        [TestMethod]
        public void testCompareToSame()
        {
            ValueComparablePair<string, int> pair = new ValueComparablePair<string, int>("a", 1);
            ValueComparablePair<string, int> pairWithSame = new ValueComparablePair<string, int>("a", 1);
            Assert.AreEqual(pair.CompareTo(pairWithSame), 0);
        }

        [TestMethod]
        public void testCompareToGreater()
        {
            ValueComparablePair<string, int> pair = new ValueComparablePair<string, int>("a", 1);
            ValueComparablePair<string, int> pairWithGreaterValue = new ValueComparablePair<string, int>("b", 2);
            Assert.AreEqual(pair.CompareTo(pairWithGreaterValue), -1);
        }
    }
}
