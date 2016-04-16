using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class Items : IEnumerable<Item>
    {
        private Dictionary<string, Item> items;

        public Items(){
            items = new Dictionary<string, Item>();
        }

        public List<string> GetAllItemsIds()
        {
            return items.Keys.ToList();
        }

        public List<Item> GetAllItems()
        {
            return items.Values.ToList();
        }

        public Item GetItemById(string itemId)
        {
            Item item;
            items.TryGetValue(itemId, out item);

            return item;
        }

        public void addUserToItem(string userId, string itemId, double rating)
        {
            Item item = GetItemById(itemId);

            if (item == null)
            {
                addItem(itemId);
                addUserToItem(userId, itemId, rating);
            }
            else
            {
                item.AddUser(userId, rating);
            }
        }



        public void addItem(string itemId)
        {
            if (items.Keys.Contains(itemId))
                throw new NotSupportedException("Item " + "[" + itemId + "]" + " already exists in the DB!");

            Item item = new Item(itemId);
            items.Add(itemId, item);
        }

        public IEnumerator<Item> GetEnumerator()
        {
            foreach (KeyValuePair<string, Item> item in items)
            {
                yield return item.Value;
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
