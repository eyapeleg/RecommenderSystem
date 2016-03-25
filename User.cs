using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class User
    {
        private Dictionary<string, double> itemsRatings;
        private UserSimilarities similarUsers;
        private List<User> intersectUserList; 
        private double sum;
        private int count;
        private string id;
        private double squaredSum;

        public User(string userId)
        {
            sum = 0.0;
            squaredSum = 0.0;
            count = 0;
            id = userId;
            itemsRatings = new Dictionary<string, double>();
            similarUsers = new UserSimilarities();
            intersectUserList = new List<User>();
        }

        public string GetId()
        {
            return id;
        }

        public double GetSquaredSum()
        {
            return squaredSum;
        }

        public IEnumerable<KeyValuePair<double, User>> GetSimilarUser(PredictionMethod method)
        {
            return similarUsers.GetSimilarUsers(method);
        }

         //public void setSimilarUser(User uID, double w)
         //{
         //    if (similarUsers.ContainsKey(uID))
         //    {
         //        throw new NotSupportedException("User " + "[" + uID + "]" + " already exists in the similar users list!");
         //    }

         //    similarUsers.Add(uID, w);
         //}*/


        public void AddItem(string item, double rating)
        {
            if (itemsRatings.ContainsKey(item))
                throw new NotSupportedException("Item " + "[" + item + "]" + " already exists in the DB!");

            sum += rating;
            squaredSum += Math.Pow(rating, 2);
            count++;
            itemsRatings.Add(item, rating);
        }

        public double GetAverageRatings()
        {
            return (sum / (double)count);
        }


        public List<string> GetRatedItems()
        {
            return itemsRatings.Keys.ToList();
        }


        public double GetRating(string sIID)
        {
            double rating;

            if (itemsRatings.TryGetValue(sIID, out rating))
                return rating;

            return 0.0;
        }

        public Dictionary<double,double> GetRatingDistribution()
        {
            double sum = 0.0;

            double totalRatedItems = itemsRatings.Count();
            var dict = itemsRatings.GroupBy(i => i.Value).ToDictionary(g => g.Key, g => g.Count() / totalRatedItems).ToDictionary(w=> w.Key, w => sum += w.Value);

            return dict;

        }

        /*public override bool Equals(object obj)
        {
            if (obj == null || !obj.GetType().IsInstanceOfType(this.GetType()))
                return false;

            if (obj == this)
                return true;

            if (((User)obj).getId().Equals(this.getId()))
                return true;

            return false;
        }

        //TODO - implement hash code*/

        public void SetIntersectUserList(User user)
        {
            intersectUserList.Add(user);
        }

        internal void SetSimilarUser(PredictionMethod method, User u2, double similarity)
        {
            if (method != null)
                switch (method)
                {
                    case PredictionMethod.Pearson:
                        similarUsers.Add(PredictionMethod.Pearson, u2, similarity);
                        break;
                    case PredictionMethod.Cosine:
                        similarUsers.Add(PredictionMethod.Cosine, u2, similarity);
                        break;
                }
        }
    }
}
