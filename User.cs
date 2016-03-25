﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class User
    {
        private Dictionary<string, double> itemsRatings;
        private Dictionary<User, double> similarUsers; 
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
            similarUsers = new Dictionary<User, double>();
        }

        public string getId()
        {
            return this.id;
        }

        public double getSquaredSum()
        {
            return squaredSum;
        }

        public Dictionary<User, double> getSimilarUser()
        {
            return similarUsers.OrderByDescending(x => x.Value).ToDictionary(w => w.Key, w => w.Value);
        }

        public void setSimilarUser(User uID, double w)
        {
            if (similarUsers.ContainsKey(uID))
            {
                throw new NotSupportedException("User " + "[" + uID + "]" + " already exists in the similar users list!");
            }

            similarUsers.Add(uID, w);
        }


        public void addItem(string item, double rating)
        {
            if (itemsRatings.ContainsKey(item))
                throw new NotSupportedException("Item " + "[" + item + "]" + " already exists in the DB!");

            sum += rating;
            squaredSum += Math.Pow(rating, 2);
            count++;
            itemsRatings.Add(item, rating);
        }

        public double getAverageRatings()
        {
            return (sum / (double)count);
        }


        public List<string> getRatedItems()
        {
            return itemsRatings.Keys.ToList();
        }


        public double getRating(string sIID)
        {
            double rating;

            if (itemsRatings.TryGetValue(sIID, out rating))
                return rating;

            return 0.0;
        }

        public Dictionary<double,double> getRatingDistribution()
        {
            double sum = 0.0;

            double totalRatedItems = itemsRatings.Count();
            var dict = itemsRatings.GroupBy(i => i.Value).ToDictionary(g => g.Key, g => g.Count() / totalRatedItems).ToDictionary(w=> w.Key, w => sum += w.Value);

            return dict;

        }

        public override bool Equals(object obj)
        {
            if (obj == null || !obj.GetType().IsInstanceOfType(this.GetType()))
                return false;

            if (obj == this)
                return true;

            if (((User)obj).getId().Equals(this.getId()))
                return true;

            return false;
        }

        //TODO - implement hash code

    }
}
