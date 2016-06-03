using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class BasedItem : IEnumerable<KeyValuePair<Item,int>>
    {
        private Dictionary<Item, int> itemsCount;
        private int totalItemCount;

        private Item givenItem;
        
        public BasedItem()
        {
            itemsCount = new Dictionary<Item, int>();
            totalItemCount = 0;
        }

        public double getConditionalProbability(Item item)
        {
            if (!itemsCount.ContainsKey(item))
                return 0.0;

            double numerator = itemsCount[item];
            double denominator = totalItemCount;

            return (double)numerator / (double)denominator;
        }

        public void removeItem(Item item)
        {
            if (!itemsCount.ContainsKey(item))
                return;

            int count = itemsCount[item];
            itemsCount.Remove(item);

            totalItemCount -= count;
        }

        public void addItem(Item item)
        {
            if (itemsCount.ContainsKey(item))
            {
                itemsCount[item]++;
            }
            else
            {
                itemsCount.Add(item, 1);
            }
            totalItemCount++;
        }

        public IEnumerator<KeyValuePair<Item,int>> GetEnumerator()
        {
            return itemsCount.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
