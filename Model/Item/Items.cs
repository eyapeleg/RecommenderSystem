using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class Items
    {
        private Dictionary<string, Dictionary<string, double>> items;

        public Items(){
            items = new Dictionary<string, Dictionary<string, double>>();
        }

        public List<string> GetAllItems()
        {
            return items.Keys.ToList();
        }

        public Dictionary<string, Dictionary<string, double>> GetUsersPerItemList()
        {
            return items;
        }

        public Dictionary<string, double> GetItemById(string sIID)
        {
            Dictionary<string, double> users;
            items.TryGetValue(sIID, out users);

            return users;
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
