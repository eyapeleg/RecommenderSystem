using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class Item
    {
        private string id;
        private Dictionary<string, double> itemsRatings;

        public Item(string id)
        {
            this.id = id;
            itemsRatings = new Dictionary<string, double>();
        }

        public string GetId()
        {
            return id;
        }

        public void AddUser(string user, double rating)
        {
            if (itemsRatings.ContainsKey(user))
                throw new NotSupportedException("User " + "[" + user + "]" + " already exists in the DB!");

            itemsRatings.Add(user, rating);
        }

        public List<string> GetRatingUsers()
        {
            return itemsRatings.Keys.ToList();
        }

        public int CompareTo(Item other)
        {
            return id.CompareTo(other.id);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !this.GetType().Name.Equals(obj.GetType().Name))
                return false;

            if (obj == this)
                return true;

            if (((Item)obj).GetId().Equals(this.GetId()))
                return true;

            return false;
        }
    }
}
