using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RecommenderSystem
{
    public class ItemToItem : IEnumerable<KeyValuePair<Tuple<Item,Item>,int>>
    {
        Dictionary<Tuple<Item, Item>, int> itemToItemCount;
        Dictionary<Item, int> itemCount;

        public ItemToItem()
        {
            itemToItemCount = new Dictionary<Tuple<Item, Item>, int>();
            itemCount = new Dictionary<Item, int>();
        }

        public double getConditionalProbability(Item givenItem, Item expectedItem)
        {
            Tuple<Item,Item> itemPair = enforcePairIdsOrder(new Tuple<Item,Item>(givenItem,expectedItem));
            if (!itemCount.ContainsKey(givenItem) || !itemCount.ContainsKey(expectedItem) || !itemToItemCount.ContainsKey(itemPair))
                return 0.0;

            double numerator = itemToItemCount[itemPair];
            double denominator = itemCount[expectedItem];

            return (double) numerator / (double)denominator;
        }

        public void addPair(Tuple<Item, Item> itemPair)
        {
            addToItemToItemCount(itemPair);
            addToItemCount(itemPair.Item1);
            addToItemCount(itemPair.Item2);
        }

        public void removePair(Tuple<Item, Item> itemPair)
        {
            if (!itemToItemCount.ContainsKey(itemPair))
                return;

            int pairCount = itemToItemCount[itemPair];
            itemToItemCount.Remove(itemPair);

            itemCount[itemPair.Item1] -= pairCount;
            itemCount[itemPair.Item2] -= pairCount;
        }

        private void addToItemToItemCount(Tuple<Item, Item> itemPair)
        {
           itemPair = enforcePairIdsOrder(itemPair);

            if (itemToItemCount.ContainsKey(itemPair))
            {
                itemToItemCount[itemPair]++;
            }
            else
            {
                itemToItemCount.Add(itemPair, 1);
            }
        }

        private Tuple<Item,Item> enforcePairIdsOrder(Tuple<Item, Item> itemPair)
        {
           if (itemPair.Item1.GetId().CompareTo(itemPair.Item2.GetId()) > 0)
              itemPair = Tuple.Create(itemPair.Item2, itemPair.Item1);
        
           return itemPair;
        }

        private void addToItemCount(Item item)
        {
            if (itemCount.ContainsKey(item)){
                itemCount[item]++;
            }
            else
            {
                itemCount.Add(item, 1);
            }

        }

        public IEnumerator<KeyValuePair<Tuple<Item,Item>,int>> GetEnumerator()
        {
            return itemToItemCount.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
