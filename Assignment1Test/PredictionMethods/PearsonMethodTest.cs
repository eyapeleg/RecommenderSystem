using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assignment1;
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

            user1.AddItem("item1", 3);
            user1.AddItem("item2", 7);
            user1.AddItem("item3", 6);
            user1.AddItem("item4", 4);
            user1.AddItem("item5", 1);
            user1.AddItem("item6", 1);
            user1.AddItem("item7", 5);

            user2.AddItem("item1", 6);
            user2.AddItem("item2", 4);
            user2.AddItem("item3", 3);
            user2.AddItem("item4", 7);
            user2.AddItem("item5", 6);
            user2.AddItem("item6", 4);
            user2.AddItem("item7", 1);

            PearsonMethod PearsonMethod = new PearsonMethod();
            string expetected = String.Format("{0:0.000}",-0.4325);
            string acutal = String.Format("{0:0.000}",PearsonMethod.calculateSimilarity(user1, user2));
            Assert.AreEqual(expetected,acutal);
        }
    }
}
