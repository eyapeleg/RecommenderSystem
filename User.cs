using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    public class User
    {
        private Dictionary<string, double> itemsRatings;

        private double sum;
        private int count;
        private string id;

        public User(string userId)
        {
            sum = 0.0;
            count = 0;
            id = userId;
        }

        public string getId()
        {
            return this.id;
        }

        public void addItem(string item, double rating)
        {
            if (itemsRatings.ContainsKey(item))
                throw new NotSupportedException("Item " + "[" + item + "]" + " already exists in the DB!");

            sum += rating;
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
