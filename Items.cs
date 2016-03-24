using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assignment1
{
    class Items
    {
        private Dictionary<string, Dictionary<string, double>> items;

        public Items(){
            items = new Dictionary<string, Dictionary<string, double>>();
        }

        public List<string> GetAllItems()
        {
            return items.Keys.ToList();
        }

        public void addUserToItems(string userId, string itemId, double rating)
        {
            if (items.ContainsKey(itemId))
            {
                items[itemId].Add(userId, rating);
            }
            else
            {
                var value = new Dictionary<string, double>() { { userId, rating } };
                items.Add(itemId, value);
            }
        }

    }
}
