using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class RecommenderSystem
    {
        Dictionary<PredictionMethod, IPredictionMethod> predictionMethodsDictionary;
        public enum PredictionMethod { Pearson, Cosine, Random };
        private Users users;
        private Items items;
        private PearsonMethod pearson;
        private CosineMethod cosine;
        private RandomMethod random;
        //class members here

        //constructor
        public RecommenderSystem()
        {
            users = new Users();
            items = new Items();
            
            predictionMethodsDictionary = new Dictionary<PredictionMethod, IPredictionMethod>(){
                {PredictionMethod.Pearson,new PearsonMethod()} , {PredictionMethod.Cosine, new CosineMethod(users, items)}
            };
        }

        public void Load(string sFileName)
        {
            DataLoader dataLoader = new DataLoader();
            Tuple<Users,Items> data = dataLoader.Load(sFileName);
            users = data.Item1;
            items = data.Item2;

            //cosine = new CosineMethod(users, items);
            //cosine.calculateCosineSimilarity();
            //var itemsArray = items.GetUsersPerItemList();
            //Dictionary<string, string> intersectDictionary = new Dictionary<string, string>();
            //for (int i = 0; i < itemsArray.Keys.Count; i++)
            //{
            //    var item1Users = itemsArray.ElementAt(i).Value.Keys;
            //    for (int j = i + 1; j < itemsArray.Keys.Count; j++)
            //    {
            //        var item2Users = itemsArray.ElementAt(j).Value.Keys;
            //        List<string> intersectList = item1Users.Intersect(item2Users).ToList();
            //        for (int k = 0; k < intersectList.Count; k++)
            //        {
            //            string userId = intersectList.ElementAt(k);
            //            for (int l = k+1; l < intersectList.Count; l++)
            //            {
            //                if (!intersectDictionary.Keys.Contains(userId))
            //                    intersectDictionary.Add(userId, intersectList.ElementAt(l));
            //                else
            //                {
            //                    intersectDictionary[userId] = intersectList.ElementAt(l);
            //                }
            //            }    
            //        }
            //    }

                //PearsonMethod pearson = new PearsonMethod();
            //User[] usersArray = users.getUsersArray();

            //for (int i = 0; i < usersArray.Length; i++)
            //{
            //    User u1 = usersArray[i];

            //    for (int j = i + 1; j < usersArray.Length; j++)
            //    {
            //        User u2 = usersArray[j];
            //        var commonRatedItems = usersArray[i].GetRatedItems().Intersect(usersArray[j].GetRatedItems());
            //        if (commonRatedItems.Any())
            //        {

            //            u1.SetIntersectUserList(u2);
            //            u2.SetIntersectUserList(u1);
            //            Console.WriteLine("User [{0}] has intersection with User [{1}]", u1.GetId(), u2.GetId());
            //            foreach (IPredictionMethod method in predictionMethodsDictionary.Values)
            //            {
            //                double similarity = method.calculateSimilarity(u1, u2);
            //                if (similarity != 0)
            //                {
            //                    u1.SetSimilarUser(method.GetPredictionMethod(), u2, similarity);
            //                    u2.SetSimilarUser(method.GetPredictionMethod(), u1, similarity);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        //return a list of the ids of all the users in the dataset
        public List<string> GetAllUsers()
        {
            return users.GetAllUsers();
        }

        //returns a list of all the items in the dataset
        public List<string> GetAllItems()
        {
            return items.GetAllItems();
        }

        //returns the list of all items that the given user has rated in the dataset
        public List<string> GetRatedItems(string sUID)
        {
            return users.GetRatedItems(sUID);
        }

        //Returns a user-item rating that appears in the dataset (not predicted)
        public double GetRating(string sUID, string sIID) 
        {
            return users.GetRating(sUID, sIID);
        }

        //predict a rating for a user item pair using the specified method
        public double PredictRating(PredictionMethod m, string sUID, string sIID)
        {
            double rating = 0.0;
            User u = users.getUserById(sUID);

            switch(m)
            {
                case PredictionMethod.Cosine:
                    //rating = cosine.PredictRating(u, sIID);
                    break;
                case PredictionMethod.Pearson:
                    break;
                case PredictionMethod.Random:
                    break;
            }
            return 0;//TODO: remove once all methods are implemented 
        }

        //Compute MAE (mean absolute error) for a set of rating prediction methods over the same user-item pairs
        //cTrials specifies the number of user-item pairs to be tested
        public Dictionary<PredictionMethod, double> ComputeMAE(List<PredictionMethod> lMethods, int cTrials)
        {
            throw new NotImplementedException();
        }
    }
}
