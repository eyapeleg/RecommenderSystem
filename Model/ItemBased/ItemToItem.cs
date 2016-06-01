using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class ItemToItem : IEnumerable<KeyValuePair<Item,int>>
    {
        private Dictionary<Item, int> itemToItemCount;
        private Item item;
        private int itemCount;

        public ItemToItem()
        {
            itemToItemCount = new Dictionary<Item, int>();
            itemCount = 0;
        }

        public double getConditionalProbability(Item item)
        {
            if (!itemToItemCount.ContainsKey(item))
                return 0.0;

            double numerator = itemToItemCount[item];
            double denominator = itemCount;

            return (double) numerator / (double)denominator;
        }


        public void removeItem(Item item)
        {
            if (!itemToItemCount.ContainsKey(item))
                return;

            int count = itemToItemCount[item];
            itemToItemCount.Remove(item);

            itemCount -= count;
        }

        public void addItem(Item item)
        {
            if (itemToItemCount.ContainsKey(item))
            {
                itemToItemCount[item]++;
            }
            else
            {
                itemToItemCount.Add(item, 1);
            }
            itemCount++;
        }

        public IEnumerator<KeyValuePair<Item,int>> GetEnumerator()
        {
            return itemToItemCount.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
