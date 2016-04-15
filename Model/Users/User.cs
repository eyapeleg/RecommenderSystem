﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class User : IComparable<User>
    {
        private Dictionary<string, double> itemsRatings;
        //private UserSimilarities similarUsers;
        //private List<User> intersectUserList;  //TODO - verify
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
        }

        public string GetId()
        {
            return id;
        }

        public double GetSquaredSum()
        {
            return squaredSum;
        }

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

        public Dictionary<string, double> GetRatedItemsDic()
        {
            return itemsRatings;
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

        public int CompareTo(User other)
        {
            return id.CompareTo(other.id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Name.Equals(obj.GetType().Name))
                return false;

            if (obj == this)
                return true;

            if (((User)obj).GetId().Equals(this.GetId()))
                return true;

            return false;
        }

        public double GetRandomRate()
        {
            Random rnd = new Random();
            var idx = rnd.Next(0,itemsRatings.Count - 1);
            return itemsRatings.ElementAt(idx).Value;
        }

        public void RemoveItemById(string itemId)
        {
            itemsRatings.Remove(itemId);
        }
    }
}
